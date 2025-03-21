-- apparently the only solution to https://github.com/evilfactory/LuaCsForBarotrauma/issues/245

-- Global var to track already patched methods
if AdditionalHooks == nil then AdditionalHooks = {} end

-- Harmony.Patch, only if not patched already
if EnsurePatch == nil then
  function EnsurePatch(class, method, params, patch, hookType)
    local combinedName = class .. "." .. method
    
    if AdditionalHooks[combinedName] == true then 
      --print(combinedName, " Already patched!")
      return 
    end
    AdditionalHooks[combinedName] = true
  
    Hook.Patch('AdditionalHooks', class, method,params, patch, hookType)
  end
end
