﻿namespace BullseyeCursors.Interfaces
{
    public interface ISubject
    {
        void Attach(IObserver observer);

        void Notify();
    }
}