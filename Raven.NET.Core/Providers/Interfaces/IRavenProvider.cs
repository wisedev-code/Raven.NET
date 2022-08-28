using System;
using System.Collections;
using System.Collections.Generic;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Providers.Interfaces
{
    /// <summary>
    /// Interface which is meant to use internally to register ravens and have little control over it
    /// </summary>
    public interface IRavenProvider
    {
        /// <summary>
        /// Method adds raven to internal collection
        /// </summary>
        /// <param name="ravenName"></param>
        /// <param name="raven"></param>
        /// <param name="ravenSubject"></param>
        void AddRaven(string ravenName, IRaven raven, Type ravenSubject = default);

        /// <summary>
        /// Method to update raven internal collection of subject
        /// </summary>
        /// <param name="ravenName"></param>
        /// <param name="observers"></param>
        /// <param name="type"></param>
        internal void UpdateSubjects(string ravenName, IEnumerable<RavenSubject> observers, Type type = default);
        
        /// <summary>
        /// Method removes raven from internal collection
        /// </summary>
        /// <param name="ravenName"></param>
        void RemoveRaven(string ravenName);

        /// <summary>
        /// Method to get raven from collection
        /// </summary>
        /// <param name="ravenName"></param>
        /// <param name="isTypedWatcher"></param>
        IRaven GetRaven(string ravenName, Type type = default);
        
        /// <summary>
        /// Methods check if raven with this name is already specified
        /// </summary>
        /// <param name="ravenName"></param>
        /// <returns></returns>
        bool RavenExist(string ravenName);
        
        /// <summary>
        /// This method can force raven update on registered subject
        /// </summary>
        /// <param name="subject"></param>
        void UpdateRavens(RavenSubject subject);
    }
}