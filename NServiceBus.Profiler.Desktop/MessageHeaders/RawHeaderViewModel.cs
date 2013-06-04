using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.MessageHeaders.Editors;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    [TypeConverter(typeof(HeaderInfoTypeConverter))]
    public class RawHeaderViewModel : HeaderInfoViewModelBase, IRawHeaderViewModel
    {
        private readonly IContentDecoder<IList<HeaderInfo>> _decoder;

        public RawHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManager queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            _decoder = decoder;
        }

        protected override void MapHeaderKeys()
        {
        }

        protected override void ClearHeaderValues()
        {
            HeaderContent = null;
        }

        [Editor(typeof(ResizableDropDownEditor), typeof(ResizableDropDownEditor))]
        [Description("Raw header information")]
        public string HeaderContent { get; set; }

        protected override IList<HeaderInfo> DecodeHeader(MessageBody message)
        {
            HeaderContent = Encoding.UTF8.GetString(message.Headers);

            var decodedResult = _decoder.Decode(message.Headers);
            return decodedResult.IsParsed ? decodedResult.Value : new HeaderInfo[0];
        }
    }
}