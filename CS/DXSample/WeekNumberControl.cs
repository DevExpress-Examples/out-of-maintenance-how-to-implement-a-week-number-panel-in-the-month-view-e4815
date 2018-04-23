using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXSample {
    public partial class WeekNumberControl : Control {

        private SchedulerControl _Scheduler;
        public SchedulerControl Scheduler {
            get {
                return _Scheduler;
            }
            set {
                if(_Scheduler == value)
                    return;

                if(_Scheduler != null) {
                    _Scheduler.VisibleIntervalChanged -= _Scheduler_VisibleIntervalChanged;
                    _Scheduler.CustomDrawTimeCell -= _Scheduler_CustomDrawTimeCell;
                }
                _Scheduler = value;
                _Scheduler.VisibleIntervalChanged += _Scheduler_VisibleIntervalChanged;
                _Scheduler.CustomDrawTimeCell += _Scheduler_CustomDrawTimeCell;
                Invalidate();
            }
        }

        DateTime Start { get; set; }
        List<RectParameters> WeekRectParameters { get; set; }

        public WeekNumberControl() {
            InitializeComponent();
            WeekRectParameters = new List<RectParameters>();
            for(int i = 0; i < 5; i++) {
                WeekRectParameters.Add(new RectParameters());
            }
        }

        void _Scheduler_CustomDrawTimeCell(object sender, CustomDrawObjectEventArgs e) {
            MonthSingleWeekCell cell = e.ObjectInfo as MonthSingleWeekCell;
            if(cell == null)
                return;

            if(cell.Interval.Start.DayOfWeek == DayOfWeek.Monday) {
                for(int i = 0; i < 5; i++) {
                    if(cell.Interval.Start == Start.AddDays(i * 7)) {
                        WeekRectParameters[i] = new RectParameters() { Y = e.Bounds.Y, Height = e.Bounds.Height };
                    }
                }
                Invalidate();
            }
        }

        void _Scheduler_VisibleIntervalChanged(object sender, EventArgs e) {
            Start = Scheduler.Start;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            if(Scheduler == null)
                return;

            Pen pen = new Pen(Brushes.Black);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            Rectangle monthRect;
            for(int i = 0; i < 5; i++) {
                monthRect = new Rectangle(new Point(pe.ClipRectangle.Location.X, WeekRectParameters[i].Y),
                    new Size(pe.ClipRectangle.Width - 1, WeekRectParameters[i].Height));
                pe.Graphics.DrawRectangle(pen, monthRect);

                int numberStart = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(Start.AddDays(7 * i), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                int numberEnd = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(Start.AddDays(7 * i + 6), CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);

                string weekNumber = "";
                if(numberStart == numberEnd) {
                    weekNumber = numberStart.ToString();
                } else {
                    weekNumber = numberStart.ToString() + " / " + numberEnd.ToString();
                }

                pe.Graphics.DrawString(weekNumber, Scheduler.Appearance.HeaderCaption.Font, Brushes.Black, monthRect, sf);
            }
        }
    }

    class RectParameters {
        public int Y { get; set; }
        public int Height { get; set; }
    }
}
