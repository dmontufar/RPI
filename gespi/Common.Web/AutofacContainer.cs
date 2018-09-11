using System;
using System.Collections.Generic;
using HttpDependencies = System.Web.Http.Dependencies;
using Autofac;
using AutofacMvc = Autofac.Integration.Mvc;

namespace Common.Web
{
    public class AutoScopeContainer : HttpDependencies.IDependencyScope
    {
        protected AutofacMvc.AutofacDependencyResolver container;

        public AutoScopeContainer(AutofacMvc.AutofacDependencyResolver container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (container.ApplicationContainer.IsRegistered(serviceType))
            {
                return this.container.GetService(serviceType);
            }
            else
            {
                return null;
            }

        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (container.ApplicationContainer.IsRegistered(serviceType))
            {
                return this.container.GetServices(serviceType);
            }
            else
            {
                return new List<object>();
            }
        }

        public void Dispose()
        {
            container.ApplicationContainer.Dispose();
        }

    }

    public class AutoFacContainer : AutoScopeContainer, HttpDependencies.IDependencyResolver
    {
        public AutoFacContainer(AutofacMvc.AutofacDependencyResolver container)
            : base(container)
        {
        }

        public HttpDependencies.IDependencyScope BeginScope()
        {
            return this;
        }
    }
}
