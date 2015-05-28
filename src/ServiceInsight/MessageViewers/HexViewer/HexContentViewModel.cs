namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;

    public class HexContentViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        public List<Tuple<int,byte[]>> Data { get; set; }

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

            var temp = new List<Tuple<int,byte[]>>(lines);
            int startIdx = 0;
            for (var i = 0; i < lines; i++)
            {
                var r = new byte[16];
                var f = 16;
                if (startIdx + 16 > body.Length)
                {
                    f = body.Length - startIdx;
                }

                Array.Copy(body, startIdx, r, 0, f);

                startIdx += 16;

                temp.Add(Tuple.Create( i+1,r));
            }
            Data = temp;
        }
    }
}