using System;
using System.Windows.Forms;

using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace Tekla.Technology.Akit.UserScript
{
    static class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            DrawingHandler drawingHandler = new DrawingHandler();

            if (drawingHandler.GetActiveDrawing() == null)
            {
                // remove from drawings selected in drawing list
                DrawingEnumerator drawingEnum = drawingHandler.GetDrawingSelector().GetSelected();
                string message = string.Format("Remove all clouds from {0} selected drawings?", drawingEnum.GetSize());
                DialogResult result = MessageBox.Show(message, "Important!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    while (drawingEnum.MoveNext())
                    {
                        Drawing drawing = drawingEnum.Current;
                        removeClouds(drawing);
                    }
                }
            }
            else
            {
                DrawingObjectEnumerator drawingoe = drawingHandler.GetDrawingObjectSelector().GetSelected();
                drawingoe.MoveNext();
                Console.WriteLine(drawingoe.Current);

                // remove from active drawing
                string message = "Remove all clouds from active drawing?";
                DialogResult result = MessageBox.Show(message, "Important!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    Drawing drawing = drawingHandler.GetActiveDrawing();
                    removeClouds(drawing);
                }
            }
        }

        private static void removeClouds(Drawing drawing)
        {
            DrawingObjectEnumerator objects = drawing.GetSheet().GetAllObjects(new Type[] { typeof(TSD.Polygon), typeof(Cloud) });

            if (objects.GetSize() > 0)
            {
                while (objects.MoveNext())
                {
                    if (objects.Current is TSD.Polygon)
                    {
                        TSD.Polygon poly = (TSD.Polygon)objects.Current;

                        if (poly.Bulge > 0)
                        {
                            poly.Delete();
                        }
                    }
                    else if (objects.Current is Cloud)
                    {
                        Cloud cloud = (Cloud)objects.Current;
                        cloud.Delete();
                    }
                }

                drawing.CommitChanges();
            }
        }

    }
}
