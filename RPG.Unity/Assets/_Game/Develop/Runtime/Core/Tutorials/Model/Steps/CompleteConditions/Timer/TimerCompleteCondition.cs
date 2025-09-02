using Newtonsoft.Json;
using PrimeTween;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Timer
{
    internal class TimerCompleteCondition : TutorialCondition
    {
        private Tween _timerTween;
        private readonly TimerConditionData _data;

        public TimerCompleteCondition(string dataJson) => 
            _data = JsonConvert.DeserializeObject<TimerConditionData>(dataJson);

        public override void Initialize()
        {
            base.Initialize();

            _timerTween = Tween.Delay(_data.Duration, Complete, useUnscaledTime: true);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _timerTween.Stop();
        }

        public override void Pause() => 
            _timerTween.isPaused = true;

        public override void Continue() => 
            _timerTween.isPaused = false;
    }
}