return (function()
    local this = {}

    function this.OneTimeSetup()
        system.print("OTS")
    end
    function this.Setup()
        system.print("TS")
    end
    function this.Cleanup()
        system.print("TC")
    end
    function this.OneTimeCleanup()
        system.print("OTC")
    end

    function this.TestWeAreWorking()
        system.print("We are working")
    end


    return this
end)()