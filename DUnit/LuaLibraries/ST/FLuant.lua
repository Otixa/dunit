fluant = (function()
    local fluant_mt = {}

    local function new(val)
        local fluant = {value=val}

        function fluant.ShouldNotBeNil()
            if fluant.value == nil then
                error(string.format("Value is nil, but was expected not to be"));
            end
            return fluant
        end
        function fluant.ShouldBeNil()
            if fluant.value ~= nil then
                error(string.format("Value is not nil, but was expected to be"));
            end
            return fluant
        end
        function fluant.ShouldBe(val)
            fluant.ShouldNotBeNil()
            if fluant.value ~= val then
                error(string.format("Value is %s, but was expected to be %s", tostring(fluant.value), tostring(val) ));
            end
            return fluant
        end
        function fluant.ShouldNotBe(val)
            fluant.ShouldNotBeNil()
            if fluant.value == val then
                error(string.format("Value is %s, but was expected not to be", tostring(fluant.value) ));
            end
            return fluant
        end
        function fluant.ShouldHaveKey(key)
            fluant.ShouldNotBeNil()
            if not fluant.value[key] then
                error(string.format("Table was expected to have key %s", tostring(key) ));
            end
            return fluant
        end
        function fluant.ShouldNotHaveKey(key)
            fluant.ShouldNotBeNil()
            if fluant.value[key] then
                error(string.format("Table was expected to not have key %s", tostring(key) ));
            end
            return fluant
        end
        function fluant.KeyShouldBe(key, value)
            fluant.ShouldNotBeNil()
            fluant.ShouldHaveKey(key)
            if fluant.value[key] ~= value then
                error(string.format("Table has key %s with value %s, was expected to have value %s", tostring(key), tostring(fluant.value[key]), tostring(value) ));
            end
            return fluant
        end
        function fluant.KeyShouldNotBe(key, value)
            fluant.ShouldNotBeNil()
            fluant.ShouldHaveKey(key)
            if fluant.value[key] == value then
                error(string.format("Table has key %s with value %s, was expected to not have value %s", tostring(key), tostring(value), tostring(fluant.value[key]) ));
            end
            return fluant
        end
        function fluant.ShouldContain(value)
            fluant.ShouldNotBeNil()
            for index, val in ipairs(fluant.value) do
                if val == value then return fluant end
            end
            error(string.format("Table was expected to contain value %s, but did not", tostring(value) ));
        end
        function fluant.ShouldNotContain(value)
            fluant.ShouldNotBeNil()
            for index, val in ipairs(fluant.value) do
                if val == value then error(string.format("Table was not expected to contain value %s, but did not", tostring(value) )); end
            end
            return fluant
        end
        function fluant.KeyShouldContain(key, value)
            fluant.ShouldNotBeNil()
            fluant.ShouldHaveKey(key)
            for index, val in ipairs(fluant.value[key]) do
                if val == value then return fluant end
            end
            error(string.format("Table was expected to contain value %s in key %s, but did not", tostring(value), tostring(key) ));
        end
        function fluant.KeyShouldNotContain(key, value)
            fluant.ShouldNotBeNil()
            fluant.ShouldHaveKey(key)
            for index, val in ipairs(fluant.value[key]) do
                if val == value then error(string.format("Table was not expected to contain value %s in key %s", tostring(value), tostring(key) )); end
            end
            return fluant
        end

        return fluant;
    end

    function fluant_mt.__call(_, val)
        return new(val)
    end

    return setmetatable({}, fluant_mt)
end)()