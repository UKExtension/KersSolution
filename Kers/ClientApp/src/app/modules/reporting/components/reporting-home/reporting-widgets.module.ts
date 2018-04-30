import { NgModule } from '@angular/core';


import { WidgetActivitiesAgentComponent } from './widgets/widget-activities-agent.component';
import { WidgetDDAssistantComponent } from './widgets/widget-dd-assistant.component';
import { WidgetDDComponent } from './widgets/widget-dd.component';
import { WidgetMyInfoComponent } from './widgets/widget-my-info.component';
import { WidgetSpecialistComponent } from './widgets/widget-specialist.component';
import { WidgetStaffAssistantComponent } from './widgets/widget-staff-assistant.component';
import { WidgetTrainingsComponent } from './widgets/widget-trainings.component';
import { SharedModule } from '../../shared/shared.module';
import { KersCalendarModule } from '../../modules/calendar/kers-calendar.module';
import { WidgetCalendarComponent } from './widgets/widget-calendar.component';



@NgModule({
  imports:      [ 
                    SharedModule
   ],
  declarations: [ 
                    WidgetActivitiesAgentComponent,
                    WidgetDDAssistantComponent,
                    WidgetDDComponent,
                    WidgetMyInfoComponent,
                    WidgetSpecialistComponent,
                    WidgetStaffAssistantComponent,
                    WidgetTrainingsComponent,
                    WidgetCalendarComponent
                ],
  exports:      [
                    WidgetActivitiesAgentComponent,
                    WidgetDDAssistantComponent,
                    WidgetDDComponent,
                    WidgetMyInfoComponent,
                    WidgetSpecialistComponent,
                    WidgetStaffAssistantComponent,
                    WidgetTrainingsComponent,
                    WidgetCalendarComponent 
                ]
})
export class ReportingWidgetsModule { }