using Blazor.Extensions.Canvas.Canvas2D;
using graph.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace graph.Pages
{
    public class LineInfo
    {
        Point[] OriginalPoints { get; set; }
        Point[] ScaledPoints { get; set; }
        Point[] CurvedPoints { get; set; }
        public bool NeedReadraw { get; private set; }
        int Shift { get; set; }
        public string BackColor { get; set; }
        public string MainColor { get; set; }
        public int AreaWidth { get; }
        public int AreaHeight { get; }
        public float MaxY { get; }
        public float MsCostInPixel { get; }
        public int NewPointTimeInterval { get; }
        public DateTime UpdateDate { get; private set; }
        public int PointsCount{get;set;}
        public LineInfo(int areaWidth, int areaHeight, float maxY,int newPointTimeInterval,int pointsCount)
        {
            PointsCount = pointsCount;
            NewPointTimeInterval = newPointTimeInterval;
            AreaWidth = areaWidth;
            AreaHeight = areaHeight;
            MaxY = maxY;
            MsCostInPixel = ((float)AreaWidth / PointsCount)/(float)NewPointTimeInterval ;
        }

        private Point[] Rescale(Point[] points)
        {
            var res = new Point[points.Length];
            float xScale = AreaWidth / points.Length;
            float yScale = AreaHeight / MaxY;
            for (int i = 0; i < points.Length; i++)
            {
                res[i] = new Point() { X = points[i].X * xScale, Y = points[i].Y * yScale+ AreaHeight/20 };
            }
            return res;
        }
        
        public void SetNewOriginalPoints(Point[] points, DateTime updateTime)
        {
            UpdateDate = updateTime;
            OriginalPoints = points;
            ScaledPoints = Rescale(OriginalPoints);
            CurvedPoints = GetCurvePoints(ScaledPoints).ToArray();
            NeedReadraw = true;
        }
        public async Task DrawLine(Canvas2DContext ctx,int crop=0)
        {            
            await ctx.BeginPathAsync();
            await ctx.SetStrokeStyleAsync(MainColor);
            await ctx.MoveToAsync(CurvedPoints[crop].X + Shift, CurvedPoints[crop].Y);
            for (var i = crop+1; i < CurvedPoints.Count()- crop; i++)
            {
                await ctx.LineToAsync(CurvedPoints[i].X + Shift, CurvedPoints[i].Y);
            }            
        }
        public async Task DrawPoints(Canvas2DContext ctx)
        {
            //await ctx.BeginPathAsync();
            for (var i = 0; i < ScaledPoints.Length; i++)
            {
                await ctx.RectAsync(ScaledPoints[i].X - 2 + Shift, ScaledPoints[i].Y - 2, 4, 4);
            }            
        }

        public List<Point> GetCurvePoints(Point[] pts, double tension = 0.5, int numOfSegments = 16)
        {
            var _pts = new List<Point>();
            var res = new List<Point>();    // clone array
            double x, y;           // our x,y coords
            double t1x, t2x, t1y, t2y; // tension vectors
            double c1, c2, c3, c4;     // cardinal points
            double st, t;       // steps based on num. of segments
            int i;
            _pts.Add(pts[0].Clone());
            _pts.AddRange(pts.Select(a => a.Clone()));
            _pts.Add(pts[pts.Length - 1].Clone());

            for (i = 1; i < (_pts.Count - 2); i++)
            {
                for (t = 0; t <= numOfSegments; t++)
                {

                    // calc tension vectors
                    t1x = (_pts[i + 1].X - _pts[i - 1].X) * tension;
                    t2x = (_pts[i + 2].X - _pts[i].X) * tension;

                    t1y = (_pts[i + 1].Y - _pts[i - 1].Y) * tension;
                    t2y = (_pts[i + 2].Y - _pts[i].Y) * tension;

                    // calc step
                    st = t / numOfSegments;

                    // calc cardinals
                    c1 = 2 * Math.Pow(st, 3) - 3 * Math.Pow(st, 2) + 1;
                    c2 = -(2 * Math.Pow(st, 3)) + 3 * Math.Pow(st, 2);
                    c3 = Math.Pow(st, 3) - 2 * Math.Pow(st, 2) + st;
                    c4 = Math.Pow(st, 3) - Math.Pow(st, 2);

                    // calc x and y cords with common control vectors
                    x = c1 * _pts[i].X + c2 * _pts[i + 1].X + c3 * t1x + c4 * t2x;
                    y = c1 * _pts[i].Y + c2 * _pts[i + 1].Y + c3 * t1y + c4 * t2y;

                    //store points in array
                    res.Add(new Point() { X = x, Y = y });
                }
            }
            return res;
        }

        internal bool IsMatch(Point[] newPoints,ILog log)
        {
            var e = 0.00001;
            if (OriginalPoints == null || newPoints.Length != OriginalPoints.Length)
                return false;
            for (int i = 0; i < newPoints.Length; i++)
            {
                if (Math.Abs(OriginalPoints[i].X - newPoints[i].X) > e || Math.Abs(OriginalPoints[i].Y - newPoints[i].Y) > e)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task EreaseLine(Canvas2DContext ctx)
        {
            if (OriginalPoints == null)
                return;
            await ctx.SetStrokeStyleAsync(BackColor);
            await ctx.SetLineWidthAsync(4);
            await DrawLine(ctx);
            await DrawPoints(ctx);
        }
        public async Task Draw(Canvas2DContext ctx, ILog log)
        {
            if (OriginalPoints == null)
                return;
            var now = DateTime.Now;
            var diffMs = (now - UpdateDate).TotalMilliseconds;
            //Shift = -(int)(diffMs * MsCostInPixel);
            var translate= -(int)(diffMs * MsCostInPixel);
            await ctx.SaveAsync();
            await ctx.TranslateAsync(translate, 0);
            //if (NeedReadraw)
            {

                await ctx.SetStrokeStyleAsync(MainColor);
                await ctx.SetLineWidthAsync(2);                
                await DrawLine(ctx, 1);
                await DrawPoints(ctx);
                NeedReadraw = false;
            }
            await ctx.RestoreAsync();
        }                
    }
}
