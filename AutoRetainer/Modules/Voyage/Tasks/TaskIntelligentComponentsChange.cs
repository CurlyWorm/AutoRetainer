﻿using AutoRetainer.Internal;
using AutoRetainer.Modules.Voyage.PartSwapper;

namespace AutoRetainer.Modules.Voyage.Tasks;

internal static class TaskIntelligentComponentsChange
{
    internal static void Enqueue(string name, VoyageType type)
    {
        VoyageUtils.Log($"Task enqueued: {nameof(TaskIntelligentComponentsChange)}, name={name}, type={type}");
        P.TaskManager.Enqueue(() =>
        {
            if (PartSwapperUtils.GetPlanInLevelRange(Data.GetAdditionalVesselData(name, type).Level) == null) return;
            var rep = PartSwapperUtils.GetIsVesselNeedsPartsSwap(name, type, out var log);
            if(rep is { Count: > 0 })
            {
                TaskChangeComponents.EnqueueImmediate(rep, name, type);
            }
            PluginLog.Debug($"Change check log: {(log.Count > 0 ? log.Join(", ") : "None")}");
        }, "IntelligentChangeTask");
    }
}
