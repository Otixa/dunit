return (function()
    local this = {}

    function this.OneTimeSetup()
        this.slots = SlotDetector.DetectSlotsInNamespace(_G)
    end
    function this.Setup()
        
    end
    function this.Cleanup()
        
    end
    function this.OneTimeCleanup()
        
    end

    function this.SlotDetectorDetectsCore()
        fluant(this.slots.Core).ShouldNotBeNil()
    end
    function this.SlotDetectorDetectsUnit()
        fluant(this.slots.Unit).ShouldNotBeNil()
    end

    return this
end)()