using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoOnDemand.UI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VideoOnDemand.Data.Data.Entities;
using AutoMapper;
using VideoOnDemand.UI.Models.DTOModels;
using VideoOnDemand.UI.Models.MembershipViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VideoOnDemand.UI.Controllers
{
    public class MembershipController : Controller
    {
        private string _userId;
        private IReadRepository _db;
        private readonly IMapper _mapper;

        // UserManager is used to get the user id from the logged in user.
        // IHttpContextAccessor contains information about the logged in user.
        // IMapper gets access to AutoMapper
        public MembershipController(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IMapper mapper, IReadRepository db)
        {
            // Save logged in user Id in private class _userId
            var user = httpContextAccessor.HttpContext.User;
            _userId = userManager.GetUserId(user);
            _mapper = mapper;
            _db = db;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Dashbosrd()
        {
            // Fetch all courses for the User and Convert them into CourseDTO objects
            var courseDtoObjects = _mapper.Map<List<CourseDTO>>(_db.GetCourses(_userId));

            // create an instance of the DashBoardViewModel and Course property
            var dashboardModel = new DashboardViewModel
            {
                Courses = new List<List<CourseDTO>>()
            };
            // dashboardModel.Courses = new List<List<CourseDTO>>();

            // Divide the CourseDTOs in the courseDtoObjects collection into sets of three, and add them to new List<CourseDTO> instances.
            var numOfRows = courseDtoObjects.Count <= 3 ? 1 : courseDtoObjects.Count / 3;
            for (var i = 0; i < numOfRows; i++)
            {
                dashboardModel.Courses.Add(courseDtoObjects.Take(3).ToList());
            }

            return View(dashboardModel);
        }

        // will display the selected course and its associated modules.
        //Each module will list the videos and downloadables associated with it. The instructor bio will also be displayed beside the module list
        [HttpGet]
        public IActionResult Course(int id)
        {
            // Get course matching the ID param passed in.
            var course = _db.GetCourse(_userId, id);

            // Call the Map method on the _mapper variable to convert the course you just fetched into a CourseDTO object
            var mappedCourseDTOs = _mapper.Map<CourseDTO>(course);

            // convert the Instructor object in the course object into an InstructorDTO object
            var mappedInstructorDTO = _mapper.Map<InstructorDTO>(course.Instructor);

            // convert the Modules collection in the course object into a List<ModuleDTO>
            var mappedModuleDTOs = _mapper.Map<List<ModuleDTO>>(course.Modules);

            // Loop over the mappedModuleDTOs collection to fetch the videos and downloads associated with the modules.
            // Use AutoMapper to convert videos and downloads in the course object’s Modules collection to List<VideoDTO> and List<DownloadDTO> collections.
            // Assign the collections to their respective properties in the loop’s current ModuleDTO.
            for (var i = 0; i < mappedModuleDTOs.Count; i++)
            {
                mappedModuleDTOs[i].Downloads =
                    course.Modules[i].Downloads.Count.Equals(0) ? null :
                     _mapper.Map<List<DownloadDTO>>(course.Modules[i].Downloads);

                mappedModuleDTOs[i].Videos =
                    course.Modules[i].Videos.Count.Equals(0) ? null :
                    _mapper.Map<List<VideoDTO>>(course.Modules[i].Videos);
            }

            // Instance of CourseViewModel : three mapped collections: mappedCourseDTOs, mappedInstructorDTO, and mappedModuleDTOs 
            // mapped to courseModel object’s Course, Instructor, and Modules properties.
            var courseModel = new CourseModelView
            {
                Course = mappedCourseDTOs,
                Instructor = mappedInstructorDTO,
                Modules = mappedModuleDTOs
            };

            return View(courseModel);
        }

        // The Video view will display the selected video, information about the video, buttons to select the next and previous videos, and an instructor bio.
        [HttpGet]
        public IActionResult Video(int id)
        {
            // Call the _db.GetVideo method to fetch the video matching the id passed in to the Video action, and the logged in user’s id.
            var video = _db.GetVideo(_userId, id);

            // Call the _db.GetCourse method to fetch the course matching the CourseId property in the video object, and the logged in user’s id.
            var course = _db.GetCourse(_userId, video.CourseId);

            // convert the Video object into a VideoDTO object
            var mappedVideoDTO = _mapper.Map<VideoDTO>(video);

            // convert the course object into a CourseDTO object.
            var mappedCourseDTO = _mapper.Map<CourseDTO>(course);

            // convert the Instructor object in the course object into an InstructorDTO object.
            var mappedInstructorDTO = _mapper.Map<InstructorDTO>(course.Instructor);

            // _db.GetVideos method to fetch all the videos matching the current module id.
            // You need this data to get the number of videos in the module, and to get the index of the current video
            var videos = _db.GetVideos(_userId, video.ModuleId).ToList();

            // Store number of videos
            var count = videos.Count();

            // Find index of the current video in the module video list. Display index and the video count to the user, in the view.
            var index = videos.IndexOf(video);

            // Fetch the id for the previous video in the module by calling the ElementAtOrDefault method on the videos collection.
            var previous = videos.ElementAtOrDefault(index - 1);
            var previousId = previous == null ? 0 : previous.Id;

            // Fetch the id, title, and thumbnail for the next video in the module by calling the ElementAtOrDefault method on the videos collection.
            var next = videos.ElementAtOrDefault(index + 1);
            var nextId = next == null ? 0 : next.Id;
            var nextTitle = next == null ? string.Empty : next.Title;
            var nextThumb = next == null ? string.Empty : next.Thumbnail;

            var videoModel = new VideoViewModel
            {
                // Assign the three mapped collections: mappedCourseDTOs, mappedInstructorDTO, and mappedVideoDTOs to the videoModel object’s Course, Instructor, and Video properties
                Video = mappedVideoDTO,
                Instructor = mappedInstructorDTO,
                Course = mappedCourseDTO,
                // Create an instance of the LessonInfoDTO for the LessonInfo property in the videoModel object and assign the variable values to its properties.
                // The LessonInfoDTO will be used with the previous and next buttons, and to display the index of the current video
                LessonInfo = new LessonInfoDTO
                {
                    LessonNumber = index + 1,
                    NumberOfLessons = count,
                    NextVideoId = nextId,
                    PreviousVideoId = previousId,
                    NextVideoTitle = nextTitle,
                    NextVideoThumbnail = nextThumb
                }
            };

            return View();
        }
    }
}
