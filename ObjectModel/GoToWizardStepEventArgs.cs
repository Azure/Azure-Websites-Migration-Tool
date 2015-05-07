// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;

namespace CompatCheckAndMigrate.ObjectModel
{
    public class GoToWizardStepEventArgs : EventArgs
    {
        public GoToWizardStepEventArgs(WizardSteps step, object state = null, bool isNavigatingBack =false)
        {
            this.GoTo = step;
            this.State = state;
            this.IsNavigatingBack = isNavigatingBack;
        }

        public bool IsNavigatingBack { get; private set; }

        public WizardSteps GoTo { get; private set; }

        public object State { get; private set; }
    }
}
