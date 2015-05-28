namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;

    public class HexContentViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        public List<HexContentLine> Data { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Hex";
        }

        public void Handle(SelectedMessageChanged @event)
        {
            if (@event.Message == null || @event.Message.Body == null)
            {
                return;
            }

            var body = Encoding.Default.GetBytes(@event.Message.Body);

            var lines = (int)Math.Ceiling(body.Length / 16.0);

            var temp = new List<HexContentLine>(lines);
            for (var i = 0; i < lines; i++)
            {
                temp.Add(new HexContentLine(body, i));
            }
            Data = temp;
        }
    }
}