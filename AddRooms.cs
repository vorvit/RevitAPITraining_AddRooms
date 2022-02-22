using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace RevitAPITraining_AddRooms
{
    [Transaction(TransactionMode.Manual)]
    public class AddRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts = new Transaction(doc, "Добавление помещений");
            ts.Start();
            ICollection<ElementId> rooms;
            foreach (Level level in listLevel)
            {
                rooms = doc.Create.NewRooms2(level);
            }
            FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            IList<ElementId> roomids = FEC.ToElementIds() as IList<ElementId>;
            foreach (ElementId roomid in roomids)
            {
                Element e = doc.GetElement(roomid);
                Room r = e as Room;
                string lName = r.Level.Name.Substring(6);
                r.Name = $"{lName}_{r.Number}";
                //UV center = GetElementCenter(e);
                doc.Create.NewRoomTag(new LinkElementId(roomid), new UV(0, 0), null);
            }

            ts.Commit();

            return Result.Succeeded;
        }

/*        private UV GetElementCenter(Element e)
        {
            BoundingBoxXYZ bounding = e.get_BoundingBox(null);
            XYZ p = (bounding.Max + bounding.Min) / 2;
            UV center = new UV (p.X, p.Y);
            return center;
        }
*/    }
}
