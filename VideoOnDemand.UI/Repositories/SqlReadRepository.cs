using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoOnDemand.Data.Data.Entities;
using VideoOnDemand.Data.Services;

namespace VideoOnDemand.UI.Repositories
{
    public class SqlReadRepository : IReadRepository
    {
        private IDbReadService _db;

        public SqlReadRepository(IDbReadService db)
        {
            _db = db;
        }

        public Course GetCourse(string userId, int courseId)
        {
            // Check that the user is allowed to access the requested course by calling the Get method on the _db service variable
            // Use the UserCourse entity to define the method’s type. Pass in the values of the userId and courseId parameters to the method and check that the result isn’t null
            var hasAccess = _db.Get<UserCourse>(userId, courseId) != null;
            // Return the default value (null) for the Course entity if the user doesn’t have access to the course.
            if (!hasAccess)
                return default(Course);

            // Fetch the course by calling the Get method on the _db service variable.
            // Pass in the value form the courseId parameter and the value true to specify that related entities should be filled with data.
            var course = _db.Get<Course>(courseId, true);

            // Iterate over the modules in the Modules property of the course and add the downloads and videos
            foreach (var module in course.Modules)
            {
                module.Downloads = _db.Get<Download>().Where(d =>
                    d.ModuleId.Equals(module.Id)).ToList();
                module.Videos = _db.Get<Video>().Where(d =>
                   d.ModuleId.Equals(module.Id)).ToList();
            }

            return course;
        }

        public IEnumerable<Course> GetCourses(string userId)
        {
            // Call the GetWithIncludes() for the UserCourses to fetch all the course id and user id combinations from the database.
            // select only the id combinations for the user id passed in to the GetCourses method by calling the Where LINQ method on the GetWithIncludes method.
            // select the Course objects included with the UserCourse entities by calling the Select()
            var courses = _db.GetWithIncludes<UserCourse>()
                .Where(uc => uc.UserId.Equals(userId))
                .Select(c => c.Course);

            return courses;
        }

        // fetch one video from the database.
        public Video GetVideo(string userId, int videoId)
        {
            // Fetch the video matching the video id in the videoId parameter passed into the GetVideo method by calling the Get method on the _db service variable.
            var video = _db.Get<Video>(videoId);

            // Check that the user is allowed to view the video belonging to the course specified by the CourseId property of the video object.
            // Return the default value for the Video entity if the user doesn’t have access.
            var hasAccess = _db.Get<UserCourse>(userId, video.CourseId) != null;
            if (!hasAccess)
                return default(Video);

            return video;
        }

        // fetch all videos associated with the logged in user
        public IEnumerable<Video> GetVideos(string userId, int moduleId = 0)
        {
            // Fetch the module matching the module id in the moduleId parameter passed into the GetVideos method by calling the Get method on the _db service variable.
            var module = _db.Get<Module>(moduleId);

            // Check that the user is allowed to view the video belonging to the course specified by the CourseId property of the video object.
            // Return the default value for a list of Video entities if the user doesn’t have access.
            var hasAccess = _db.Get<UserCourse>(userId, module.CourseId) != null;
            if (!hasAccess)
                return default(IEnumerable<Video>);

            // Fetch the videos by calling the Get method on the _db service variable and filter on the moduleId parameter value with the Where LINQ method
            var videos = _db.Get<Video>().Where(v =>
                v.ModuleId.Equals(moduleId));

            return videos;
        }
    }
}
