﻿using AutoRetainer.Scheduler.Handlers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoRetainer.Scheduler.Tasks;

internal static unsafe class TaskDepositGil
{
    internal static bool forceCheck = false;

    private static bool HasGil => Gil > 0 || forceCheck;
    internal static int Gil => InventoryManager.Instance()->GetInventoryItemCount(1);
    internal static void Enqueue(int percent, bool isGilAmount = false)
    {
        Func<int, bool?> depFunc = isGilAmount ? RetainerHandlers.SetDepositGilAmountExact : RetainerHandlers.SetDepositGilAmount;
        P.TaskManager.Enqueue(NewYesAlreadyManager.WaitForYesAlreadyDisabledTask);
        if (C.RetainerMenuDelay > 0)
        {
            TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
        }
        P.TaskManager.Enqueue(() => HasGil == false ? true : RetainerHandlers.SelectEntrustGil());
        P.TaskManager.Enqueue(() => HasGil == false ? true : GenericHandlers.Throttle(500));
        P.TaskManager.Enqueue(() => HasGil == false ? true : GenericHandlers.WaitFor(500));
        P.TaskManager.Enqueue(() => HasGil == false ? true : RetainerHandlers.SwapBankMode());
        P.TaskManager.Enqueue(() => HasGil == false ? true : depFunc(percent));
        P.TaskManager.Enqueue(() => HasGil == false ? true : RetainerHandlers.ProcessBankOrCancel());
        P.TaskManager.Enqueue(() => { forceCheck = false; return true; });
    }
}
