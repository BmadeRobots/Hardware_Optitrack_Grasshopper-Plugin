using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Reflection;
using System.Runtime.Versioning;

namespace OptitrackGH
{
    public class OptitrackGHComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public OptitrackGHComponent()
          : base("OptitrackGHPlane", "OptiTrackPlane",
            "Description",
            "COMPAS FAB", "OptiTrack")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("pose","pose","PoseStamp from ROS",GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("plane", "plane", "output plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {            

            dynamic pose = null;
            if (!DA.GetData(0, ref pose)) { return; }
            if (pose == null) { return; }


            dynamic dynPose = pose.Value;

            var position = dynPose.pose.position;          
            var orientation = dynPose.pose.orientation;
            double x = position.x * 1000;
            double y = position.y * 1000;
            double z = position.z * 1000;

            Tuple<double, double, double> euler = q_to_euler(orientation.x, orientation.y, orientation.z, orientation.w);
            //    Rhino.RhinoApp.WriteLine("Rx:" + euler.Item1 + " Ry:" + euler.Item2 + " Rz:" + euler.Item3 );

            var p = new Plane(new Point3d(x, y, z), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            p.Transform(Transform.Rotation(euler.Item3, p.Origin));
            p.Transform(Transform.Rotation(euler.Item2, p.YAxis, p.Origin));
            p.Transform(Transform.Rotation(euler.Item1, p.XAxis, p.Origin));

            DA.SetData(0, new Plane(p));
            
            //Plane = p;
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("5449289f-dc38-4545-abd4-d9697e7a612b");



        private Tuple<double, double, double> q_to_euler(double x, double y, double z, double w)
        {
            double sinr_cosp = 2 * (w * x + y * z);
            double cosr_cosp = 1 - 2 * (x * x + y * y);
            double rx = Math.Atan2(sinr_cosp, cosr_cosp);

            double sinp = Math.Sqrt(1 + 2 * (w * y - x * z));
            double cosp = Math.Sqrt(1 - 2 * (w * y - x * z));
            double ry = 2 * Math.Atan2(sinp, cosp) - Math.PI / 2;

            double siny_cosp = 2 * (w * z + x * y);
            double cosy_cosp = 1 - 2 * (y * y + z * z);

            double rz = Math.Atan2(siny_cosp, cosy_cosp);

            return Tuple.Create(rx, ry, rz);

        }

    }


}