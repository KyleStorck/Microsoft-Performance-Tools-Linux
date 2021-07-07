﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Performance.SDK;
using Microsoft.Performance.SDK.Extensibility;
using Microsoft.Performance.SDK.Extensibility.DataCooking;
using Microsoft.Performance.SDK.Extensibility.DataCooking.SourceDataCooking;
using Microsoft.Performance.SDK.Processing;
using System.Collections.Generic;
using System.Threading;
using PerfettoCds.Pipeline.Events;
using PerfettoProcessor;

namespace PerfettoCds
{
    /// <summary>
    /// Cooks the data from the Slice table in Perfetto traces
    /// </summary>
    public sealed class PerfettoSliceCooker : BaseSourceDataCooker<MyNewEvent, PerfettoSourceParser, string>
    {
        public override string Description => "Processes events from the slice Perfetto SQL table";

        //
        //  The data this cooker outputs. Tables or other cookers can query for this data
        //  via the SDK runtime
        //
        [DataOutput]
        public ProcessedEventData<PerfettoSliceEvent> SliceEvents { get; }

        // Instructs runtime to only send events with the given keys this data cooker
        public override ReadOnlyHashSet<string> DataKeys =>
            new ReadOnlyHashSet<string>(new HashSet<string> { PerfettoPluginConstants.SliceEvent });


        public PerfettoSliceCooker() : base(PerfettoPluginConstants.SliceCookerPath)
        {
            this.SliceEvents = new ProcessedEventData<PerfettoSliceEvent>();
        }

        public override DataProcessingResult CookDataElement(MyNewEvent perfettoEvent, PerfettoSourceParser context, CancellationToken cancellationToken)
        {
            this.SliceEvents.AddEvent((PerfettoSliceEvent)perfettoEvent.SqlEvent);

            return DataProcessingResult.Processed;
        }

        public override void EndDataCooking(CancellationToken cancellationToken)
        {
            base.EndDataCooking(cancellationToken);
            this.SliceEvents.FinalizeData();
        }
    }
}
