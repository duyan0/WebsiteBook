using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSach.DesignPatterns.PrototypePattern
{
    public interface IPrototype<T>
    {
        T Clone();
    }
}