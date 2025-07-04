using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.IO;

[assembly: CommandClass(typeof(FurnitureExtractor.MyCommands))]

namespace FurnitureExtractor
{
    public class MyCommands
    {
        [CommandMethod("ExtractFurnitureData")]
        public void ExtractFurnitureData()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                using (StreamWriter writer = new StreamWriter("furniture-data.csv"))
                {
                    writer.WriteLine("Handle,Type,Layer,PositionX,PositionY,PositionZ");

                    foreach (ObjectId objId in btr)
                    {
                        Entity ent = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                        if (ent != null && ent.Layer.ToLower().Contains("furniture"))
                        {
                            string type = ent.GetType().Name;
                            string handle = ent.Handle.ToString();
                            string layer = ent.Layer;

                            string pos = "";
                            if (ent is BlockReference blockRef)
                            {
                                Point3d pos3d = blockRef.Position;
                                pos = $"{pos3d.X},{pos3d.Y},{pos3d.Z}";
                            }

                            writer.WriteLine($"{handle},{type},{layer},{pos}");
                        }
                    }
                }

                tr.Commit();
            }
        }
    }
}
