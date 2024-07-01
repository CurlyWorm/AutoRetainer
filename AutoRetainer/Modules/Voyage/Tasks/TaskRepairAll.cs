﻿using AutoRetainer.Internal;
using ECommons.Throttlers;

namespace AutoRetainer.Modules.Voyage.Tasks;

internal static unsafe class TaskRepairAll
{
    internal static volatile bool Abort = false;
    internal static string Name = "";
    internal static VoyageType Type = 0;
    internal static void EnqueueImmediate(List<int> indexes, string vesselName, VoyageType type)
    {
        VoyageUtils.Log($"Task enqueued: {nameof(TaskRepairAll)}");
        Name = vesselName;
        Type = type;
        Abort = false;
        var vesselIndex = VoyageUtils.GetVesselIndexByName(vesselName, type);
        P.TaskManager.EnqueueImmediate(() => Utils.TrySelectSpecificEntry(Lang.WorkshopRepair, () => EzThrottler.Throttle("RepairAllSelectRepair")), "RepairAllSelectRepair");
        foreach (var index in indexes)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException(nameof(index));
            P.TaskManager.EnqueueImmediate(() => VoyageScheduler.TryRepair(index), $"Repair {index}");
            P.TaskManager.EnqueueImmediate(() => Abort || VoyageScheduler.WaitForYesNoDisappear() == true, 5000, false, "WaitForYesNoDisappear");
            P.TaskManager.EnqueueImmediate(() => Abort || VoyageUtils.GetVesselComponent(vesselIndex, type, index)->Condition > 0, "WaitUntilRepairComplete");
            P.TaskManager.DelayNextImmediate(C.FrameDelay * 2, true);
        }
        P.TaskManager.EnqueueImmediate(VoyageScheduler.CloseRepair);
        //P.TaskManager.Enqueue(() => Abort ? VoyageScheduler.SelectQuitVesselMenu() : true, "SelectQuitVesselMenu failed repair");
    }
}
