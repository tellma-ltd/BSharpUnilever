using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BSharpUnilever.Controllers.Util
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Checks whether a certain type has a certain property name defined
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type type, string propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Returns an ordered query using a dynamic string key,
        /// useful when the orderby key is a user input
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="isDescending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string key, bool isDescending = false)
        {
            // Create the key selector dynamically using LINQ expressions
            var vmType = typeof(TSource);
            var param = Expression.Parameter(vmType);
            var memberAccess = Expression.MakeMemberAccess(param, vmType.GetProperty(key));
            var body = Expression.Convert(memberAccess, typeof(object)); // To handle unboxing of e.g. int members
            Expression<Func<TSource, object>> keySelector = Expression.Lambda<Func<TSource, object>>(body, param);

            // Return an ordered query, taking into account the "isDescending" parameter
            return isDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
        }

        /// <summary>
        /// Retrieves the username of the authenticated claims principal
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string UserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Extracts all errors inside an IdentityResult and concatenates them together, 
        /// falling back to a default message if no errors were found in the IdentityResult object
        /// </summary>
        /// <param name="result"></param>
        /// <param name="defaultMessage"></param>
        /// <returns></returns>
        public static string ErrorMessage(this IdentityResult result, string defaultMessage)
        {
            string errorMessage = defaultMessage;
            if (result.Errors.Any())
                errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));

            return errorMessage;
        }
    }
}
