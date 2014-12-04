using System;

namespace btnet.Security
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class PageAllowAnonymous : Attribute
    {        
    }
}