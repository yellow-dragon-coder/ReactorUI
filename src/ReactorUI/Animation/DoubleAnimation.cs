﻿using ReactorUI.Contracts;
using System;

namespace ReactorUI.Animation
{
    public class DoubleAnimation : Animation
    {
        public double? From { get; }
        public double To { get; }

        public DoubleAnimation(double from, double to, int duration, Func<double, double> easingFunction, bool reverse = false, bool loop = false, bool keepTargetValue = true)
            : base(duration, easingFunction, reverse, loop, keepTargetValue)
        {
            From = from;
            To = to;
        }

        public DoubleAnimation(double to, int duration, Func<double, double> easingFunction, bool reverse = false, bool loop = false, bool keepTargetValue = true)
            : base(duration, easingFunction, reverse, loop, keepTargetValue)
        {
            To = to;
        }
    }
}
