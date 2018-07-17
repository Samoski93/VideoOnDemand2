using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoOnDemand.Data.Data.Entities;

namespace VideoOnDemand.UI.Repositories
{
    public class MockReadRepository : IReadRepository
    {
        #region Mock Data
        List<Course> _courses = new List<Course> {
            new Course { Id = 1, InstructorId = 1,
                MarqueeImageUrl = "/images/laptop.jpg",
                ImageUrl = "/images/course.jpg", Title = "C# For Beginners",
                Description = "Course 1 Description: A very very long description."
            },
            new Course { Id = 2, InstructorId = 1,
                MarqueeImageUrl = "/images/laptop.jpg",
                ImageUrl = "/images/course2.jpg", Title = "Programming C#",
                Description = "Course 2 Description: A very very long description."
            },
            new Course { Id = 3, InstructorId = 2,
                MarqueeImageUrl = "/images/laptop.jpg",
                ImageUrl = "/images/course3.jpg", Title = "MVC 5 For Beginners",
                Description = "Course 3 Description: A very very long description."
            }
        };

        List<UserCourse> _userCourses = new List<UserCourse>
        {
            new UserCourse { UserId = "3fcd8c17-0a83-4c70-8b1c-9b2d4131a92f",
                CourseId = 1 },
            new UserCourse { UserId = "00000000-0000-0000-0000-000000000000",
                CourseId = 2 },
            new UserCourse { UserId = "3fcd8c17-0a83-4c70-8b1c-9b2d4131a92f",
                CourseId = 3 },
            new UserCourse { UserId = "00000000-0000-0000-0000-000000000000",
                CourseId = 1 }
        };

        List<Module> _modules = new List<Module>
        {
            new Module { Id = 1, Title = "Module 1", CourseId = 1 },
            new Module { Id = 2, Title = "Module 2", CourseId = 1 },
            new Module { Id = 3, Title = "Module 3", CourseId = 2 }
        };

        List<Download> _downloads = new List<Download>
        {
            new Download{Id = 1, ModuleId = 1, CourseId = 1,
                Title = "ADO.NET 1 (PDF)",
                Url = "https://1drv.ms/b/s!AuD5OaH0ExAwn48rX9TZZ3kAOX6Peg" },
            new Download{Id = 2, ModuleId = 1, CourseId = 1,
                Title = "ADO.NET 2 (PDF)",
                Url = "https://1drv.ms/b/s!AuD5OaH0ExAwn48rX9TZZ3kAOX6Peg" },
            new Download{Id = 3, ModuleId = 3, CourseId = 2,
                Title = "ADO.NET 1 (PDF)",
                Url = "https://1drv.ms/b/s!AuD5OaH0ExAwn48rX9TZZ3kAOX6Peg" }
        };
        List<Instructor> _instructors = new List<Instructor>
        {
            new Instructor{ Id = 1, Name = "John Doe",
                Thumbnail = "/images/Ice-Age-Scrat-icon.png",
                Description = "Lorem ipsum dolor sit amet, consectetur elit."
            },
             new Instructor{ Id = 2, Name = "Jane Doe",
                 Thumbnail = "/images/Ice-Age-Scrat-icon.png",
                 Description = "Lorem ipsum dolor sit, consectetur adipiscing."
             }
        };

        List<Video> _videos = new List<Video>
        {
            new Video { Id = 1, ModuleId = 1, CourseId = 1, Position = 1,
                Title = "Video 1 Title", Description = "Video 1 Description: A very very long description.",
                Duration = 50, Thumbnail = "/images/video1.jpg", Url = "https://www.youtube.com/watch?v=BJFyzpBcaCY"
            },
            new Video { Id = 2, ModuleId = 1, CourseId = 1, Position = 2,
                Title = "Video 2 Title", Description = "Video 2 Description: A very very long description.",
                Duration = 45, Thumbnail = "/images/video2.jpg", Url = "https://www.youtube.com/watch?v=BJFyzpBcaCY"
            },
            new Video { Id = 3, ModuleId = 3, CourseId = 2, Position = 1,
                Title = "Video 3 Title", Description = "Video 3 Description: A very very long description.",
                Duration = 41, Thumbnail = "/images/video3.jpg", Url = "https://www.youtube.com/watch?v=BJFyzpBcaCY"
            },
            new Video { Id = 4, ModuleId = 2, CourseId = 1, Position = 1,
                Title = "Video 4 Title", Description = "Video 4 Description: A very very long description.",
                Duration = 42, Thumbnail = "/images/video4.jpg", Url = "https://www.youtube.com/watch?v=BJFyzpBcaCY"
            },
            new Video { Id = 5, ModuleId = 1, CourseId = 1, Position = 3,
                Title = "Video 4 Title", Description = "Video 5 Description: A very very long description.",
                Duration = 91, Thumbnail = "/images/video5.jpg", Url = "https://www.youtube.com/watch?v=BJFyzpBcaCY"
            }
        };

        #endregion

        // return an IEnumerable of Customer entities
        public IEnumerable<Course> GetCourses(string userId)
        {
            // targets the _userCourses list for the logged in user, and join in the _courses list to get to the courses
            var courses = _userCourses.Where(uc => uc.UserId.Equals(userId))
                .Join(_courses, uc => uc.CourseId, c => c.Id,
                    (uc, c) => new { Course = c })
                .Select(s => s.Course);

            // With the user’s courses in a list, you can add the instructor and modules by looping through it and using LINQ to fetch the appropriate data.
            // The course objects have an instructor id, and the modules have a course id assigned to them.
            foreach (var course in courses)
            {
                course.Instructor = _instructors.SingleOrDefault(
                    s => s.Id.Equals(course.InstructorId));
                course.Modules = _modules.Where(
                    m => m.CourseId.Equals(course.Id)).ToList();
            }

            return courses;
        }

        // The purpose of this method is to return a specific course to a user when the button in one of the course panels is clicked
        public Course GetCourse(string userId, int courseId)
        {
            // Fetch a single course using _userCourses and _courses list
            var course = _userCourses.Where(uc => uc.UserId.Equals(userId))
                 .Join(_courses, uc => uc.CourseId, c => c.Id, (uc, c) => new { Course = c })
                 .SingleOrDefault(s => s.Course.Id.Equals(courseId)).Course;

            // fetch the instructor and assign the result to the Instructor property. Use the InstructorId property in the course object.
            course.Instructor = _instructors.SingleOrDefault(s => s.Id.Equals(course.InstructorId));

            // fetch the course modules and assign the result to the Modules property.
            course.Modules = _modules.Where(m => m.CourseId.Equals(course.Id)).ToList();

            // fetch the downloads and videos for each module, and assign the results to the Downloads and Videos properties respectively on each module instance.
            foreach (var module in course.Modules)
            {
                module.Downloads = _downloads.Where(d => d.ModuleId.Equals(module.Id)).ToList();
                module.Videos = _videos.Where(v => v.ModuleId.Equals(module.Id)).ToList();
            }

            return course;
        }

        // The purpose of this method is to return a specific video that the user requests by clicking on a video in one of the Course view’s module lists.
        public Video GetVideo(string userId, int videoId)
        {
            // fetch a single video using the _courses and _userCourses list - store result in video var
            var video = _videos
                 .Where(v => v.Id.Equals(videoId))
                 .Join(_userCourses, v => v.CourseId, uc => uc.CourseId, (v, uc) => new { Video = v, UserCourse = uc })
                 .Where(vuc => vuc.UserCourse.UserId.Equals(userId))
                 .FirstOrDefault().Video;

            return video;
        }

        // GetVideos. It takes a user id and an optional module id as parameters, and returns a list of Video objects.
        public IEnumerable<Video> GetVideos(string userId, int moduleId = 0)
        {
            // Fetch all videos for the logged in user, using the _videos and _userCourses lists.
            var videos = _videos
                .Join(_userCourses, v => v.CourseId, uc => uc.CourseId, (v, uc) => new { Video = v, UserCourse = uc })
                .Where(vuc => vuc.UserCourse.UserId.Equals(userId));

            // Return all the videos in the videos collection if the module id is 0 (which is the default value for the int data type),
            // otherwise return only the videos in the videos collection that match the module id.
            return moduleId.Equals(0) ?
                videos.Select(s => s.Video) :
                videos.Where(v => v.Video.ModuleId.Equals(moduleId)).Select(s => s.Video);
        }
    }
}
