using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VideoOnDemand.Data.Data;

/// <summary>
/// This service will be used from the UI and Admin projects to read data from the database.
/// </summary>

// A data service that communicates with the database tables.
// Some methods will use reflection to fetch intrinsic entities – properties in an entity that references other entities, essentially reading data from related tables.
namespace VideoOnDemand.Data.Services
{
    public class DbReadService : IDbReadService
    {
        private VODContext _db;

        // Get access to the database through VODContext
        public DbReadService(VODContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This method will return the names of all entity properties in an entity class. The names are then used to load these entities when the parent entity is loaded into memory.
        /// method examines all properties in the entity defining the method, return the names of all properties that correspond to DbSet properties in the VODContext class, which are entities.
        /// To fetch the names, you first have to find the DbSets in the VODContext class by calling the GetProperties method on the type.
        /// reflect over that class(VODContext) and fetch the names of all properties that are public and of instance type, and where the property type name contains DbSet.
        /// separate the collection properties from the reference (class) properties and return their names in two different collections from the GetEntityNames method (use tuples)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        private (IEnumerable<string> collections, IEnumerable<string> references) GetEntityNames<TEntity>() where TEntity : class
        {
            // Add the DbSets defined in the VODContext class. Use the GetProperties method on the class’s type,
            // and the property’s PropertyType.Name property in a Where LINQ method to fetch only the properties whose type name contains DbSet.
            var dbsets = typeof(VODContext)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(z => z.PropertyType.Name.Contains("DbSet"))          // fetch only the properties whose type name contains DbSet.
                .Select(z => z.Name);

            // Get the names of all the properties (tables) in the generic
            // type T that is represented by a DbSet
            var properties = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Fetch all intrinsic entity collection properties in the entity (TEntity) (property is defined as a DbSet in the VODContext class.)
            var collections = properties
                .Where(l => dbsets.Contains(l.Name))
                .Select(s => s.Name);

            // Fetch all intrinsic entity reference properties (a class, not a collection) in the entity (TEntity). Use the entities stored in the
            // dbsets collection that you fetched earlier to make sure that the property is defined as a DbSet in the VODContext class.
            // add an “s” at the end of the property name since a class property is declared without a plural “s” in the entity(i.e.Instructor),
            // but the DbSet names are declared with the plural “s” in the VODContext class.
            var classes = properties
                .Where(c => dbsets.Contains(c.Name + "s"))
                .Select(s => s.Name);

            return (collections: collections, references: classes);
        }

        /// <summary>
        /// These methods will be implemented as generic methods that can handle any entity and therefore fetch data from any table in the database.
        /// </summary>

        // choose the table to read from by defining the desired entity for the method when it is called.
        // Since the method will return all records in the specified table, no parameters are necessary.
        // (IQueryable means that query can be expanded without fetching any data)
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class
        {
            // Use the Set method on the _db context with the generic TEntity type to access the table associated with the defining entity.
            return _db.Set<TEntity>();
        }

        // return a single record from the specified table using the id passed-in through the id parameter
        public TEntity Get<TEntity>(int id, bool includedRelatedEntities = false) where TEntity : class
        {
            var record = _db.Set<TEntity>().Find(new object[] { id });

            // check that the record variable isn’t null and that the includeRelatedRecords parameter is true.
            // If the criteria are met, the intrinsic DbSet properties should be loaded and filled with data.
            if (record != null && includedRelatedEntities)
            {
                // fetch the names of the DbSet properties (intrinsic entities) in the TEntity type
                var entities = GetEntityNames<TEntity>();

                // Eager load all the tables referenced by the generic type T
                // Iterate over the names in the collections collection and load the entities for the names in the collection
                foreach (var entity in entities.collections)
                    _db.Entry(record).Collection(entity).Load();

                // Iterate over the names in the references collection and load the entities for the names in the collection
                foreach (var entity in entities.references)
                    _db.Entry(record).Reference(entity).Load();

            }
            return record;
        }

        // This overload (version) of the Get method will return a single record with a composite primary key from the specified table
        public TEntity Get<TEntity>(string userId, int id) where TEntity : class
        {
            // Fetch the record from the table defined by the TEntity type and the passed-in ids;
            var record = _db.Set<TEntity>().Find(new object[] { userId, id });
            return record;
        }

        // return a collection of records for the entity defining the method along with their related records
        public IEnumerable<TEntity> GetWithIncludes<TEntity>() where TEntity : class
        {
            // fetch the names of all intrinsic entity properties
            var entitiesNames = GetEntityNames<TEntity>();

            // Use the Set method on the _db context with the generic TEntity type to access the table associated with the defining entity
            var dbset = _db.Set<TEntity>();

            // Merge names from the collections and references collections returned from the GetEntityNames method (Union)
            var entities = entitiesNames.collections.Union(entitiesNames.references);

            // Iterate over the names and load the entities corresponding to the names. Use the Include and Load methods on the Set method to load the entities.
            foreach (var entity in entities)
                _db.Set<TEntity>().Include(entity).Load();

            return dbset;
        }

        // Converting an Entity List to a SelectedList Items (GetSelectList)
        // return a collection of SelectList items from the entity (table) defining the method.
        // SelectList items are used when displaying data in drop-down controls in a Razor Page or a MVC view.
        // valueField (String), holds the name of the property that will represent the value of each item in the drop-down (usually an id),
        // textField (String), holds the name of the property that will repre-sent the text to display for each item in the drop-down.
        public SelectList GetSelectList<TEntity>(string valueField, string textField) where TEntity : class
        {
            // Call the Get method that returns an IQueryable<TEntity> and store the result in a variable called items.
            // Then create an instance of the SelectList class and pass in the items collection and valueField and textField parameters
            var items = Get<TEntity>();
            return new SelectList(items, valueField, textField);
        }

        /// <summary>
        /// Returns the necessary statistics for the dashboard from the database.
        /// display the number of records stored in the entity tables in the database on the cards,
        /// the method return a tuple containing the values.
        /// </summary>
        public (int courses, int downloads, int instructors, int modules, int videos, int users, int userCourses) Count()
        {
            return (
                courses: _db.Courses.Count(),
                downloads: _db.Downloads.Count(),
                instructors: _db.Instructors.Count(),
                modules: _db.Modules.Count(),
                videos: _db.Videos.Count(),
                users: _db.Users.Count(),
                userCourses: _db.UserCourses.Count());
        }
    }
}
