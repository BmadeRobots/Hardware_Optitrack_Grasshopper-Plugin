using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace OptitrackGH
{
    public class OptitrackGHInfo : GH_AssemblyInfo
    {
        public override string Name => "OptitrackGH_Plane";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("9194181d-2ed3-4c79-ad99-260dcfda62cb");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}