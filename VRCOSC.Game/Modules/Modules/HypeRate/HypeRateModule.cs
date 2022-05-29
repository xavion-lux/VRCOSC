﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Graphics;

#pragma warning disable CS8618

namespace VRCOSC.Game.Modules.Modules;

public class HypeRateModule : Module
{
    public override string Title => "HypeRate";
    public override string Description => "Sends HypeRate.io heartrate values";
    public override string Author => "VolcanicArts";
    public override Colour4 Colour => Colour4.OrangeRed.Darken(0.5f);
    public override ModuleType Type => ModuleType.Health;

    private HypeRateProvider hypeRateProvider;

    public override void CreateAttributes()
    {
        CreateSetting(HypeRateSettings.Id, "HypeRate ID", "Your HypeRate ID given on your device", string.Empty);
        CreateSetting(HypeRateSettings.APIKey, "API Key", "Your API key from HypeRate", string.Empty);

        CreateParameter(HypeRateParameter.HeartrateEnabled, "Heartrate Enabled", "Whether this module is attempting to emit values", "/avatar/parameters/HeartrateEnabled");
        CreateParameter(HypeRateParameter.HeartrateNormalised, "Heartrate Normalised", "The heartrate value normalised to 60bpm", "/avatar/parameters/HeartrateNormalised");
        CreateParameter(HypeRateParameter.HeartrateUnits, "Heartrate Units", "The units value of the heartrate value", "/avatar/parameters/HeartrateUnits");
        CreateParameter(HypeRateParameter.HeartrateTens, "Heartrate Tens", "The tens value of the heartrate value", "/avatar/parameters/HeartrateTens");
        CreateParameter(HypeRateParameter.HeartrateHundreds, "Heartrate Hundreds", "The hundreds value of the heartrate value", "/avatar/parameters/HeartrateHundreds");
    }

    protected override void OnStart()
    {
        SendParameter(HypeRateParameter.HeartrateEnabled, false);
        hypeRateProvider = new HypeRateProvider(GetSettingAs<string>(HypeRateSettings.Id), GetSettingAs<string>(HypeRateSettings.APIKey));
        hypeRateProvider.OnHeartRateUpdate += handleHeartRateUpdate;
        hypeRateProvider.OnConnected += () => SendParameter(HypeRateParameter.HeartrateEnabled, true);
        hypeRateProvider.OnDisconnected += () => SendParameter(HypeRateParameter.HeartrateEnabled, false);
        hypeRateProvider.Connect();
    }

    private void handleHeartRateUpdate(int heartrate)
    {
        var normalisedHeartRate = heartrate / 60.0f;
        var individualValues = ToDigitArray(heartrate, 3);

        SendParameter(HypeRateParameter.HeartrateEnabled, true);
        SendParameter(HypeRateParameter.HeartrateNormalised, normalisedHeartRate);
        SendParameter(HypeRateParameter.HeartrateUnits, individualValues[2]);
        SendParameter(HypeRateParameter.HeartrateTens, individualValues[1]);
        SendParameter(HypeRateParameter.HeartrateHundreds, individualValues[0]);
    }

    protected override void OnStop()
    {
        hypeRateProvider.Disconnect();
    }
}

public enum HypeRateSettings
{
    Id,
    APIKey
}

public enum HypeRateParameter
{
    HeartrateEnabled,
    HeartrateNormalised,
    HeartrateUnits,
    HeartrateTens,
    HeartrateHundreds
}
