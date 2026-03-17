namespace AniDrag.Core
{
    public interface ISettingsService
    {
        float MasterVolume { get; set; }
        float MusicVolume { get; set; }
        float UIVolume { get; set; }
        float SensitivityVertical { get; set; }
        float SensitivityHorizontal { get; set; }
        float FOV { get; set; }
        bool InvertVertical { get; set; }

        event System.Action OnSettingsChanged;
    }
}