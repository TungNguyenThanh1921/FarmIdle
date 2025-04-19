using System;
using CoreBase;

public class FakeTimeProvider : ITimeProvider
{
    private DateTime fixedNow;
    public FakeTimeProvider(DateTime fixedNow)
    {
        this.fixedNow = fixedNow;
    }

    public DateTime Now => fixedNow;
}