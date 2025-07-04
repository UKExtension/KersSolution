import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import {SharedModule} from './shared/shared.module';

import {NavigationService} from './components/reporting-navigation/navigation.service';
import {NavmenuSectionComponent} from './components/reporting-navigation/navmenu-section.component';
import {NavmenuGroupComponent} from './components/reporting-navigation/navmenu-group.component';
import {NavmenuItemComponent} from './components/reporting-navigation/navmenu-item.component';
import {TopNavComponent} from './components/reporting-navigation/topnav.component';

import {ProfileService} from './components/reporting-profile/profile.service';
import {ProfileListComponent} from './components/reporting-profile/profile-list.component';
import {ProfileCurrentComponent} from './components/reporting-profile/profile-current.component';
import {ReportingProfileEditComponent} from './components/reporting-profile/reporting-profile-edit.component';
import {ProfileListDetailComponent} from './components/reporting-profile/profile-list-detail.component';

import {ReportingComponent} from './components/reporting/reporting.component';
import {ReportingHomeComponent} from './components/reporting/reporting-home.component';
import {ReportingListComponent} from './components/reporting/reporting-list.component';
import {ReportingDetailComponent} from './components/reporting/reporting-detail.component';
import {ReportingWidgetsComponent} from './components/reporting/reporting-widgets.component';
import {ReportingAlertComponent} from './components/reporting/reporting-alert.component';


import {AdminModule} from './modules/admin/admin.module';

import {ReportingService} from './components/reporting/reporting.service';

import {ReportingRoutingModule} from './reporting-routing.module';
import { ReportingHelpHomeComponent } from './components/reporting-help/reporting-help-home.component';
import { WidgetActivitiesAgentComponent } from './components/reporting-home/widgets/widget-activities-agent.component';
import { WidgetActivitiesProgramAssistantComponent } from './components/reporting-home/widgets/widget-activities-program-assistant.component';
import { WidgetDDAssistantComponent } from './components/reporting-home/widgets/widget-dd-assistant.component';
import { WidgetDDComponent } from './components/reporting-home/widgets/widget-dd.component';
import { WidgetMyInfoComponent } from './components/reporting-home/widgets/widget-my-info.component';
import { WidgetSpecialistComponent } from './components/reporting-home/widgets/widget-specialist.component';
import { WidgetStaffAssistantComponent } from './components/reporting-home/widgets/widget-staff-assistant.component';
import { WidgetTrainingsComponent } from './components/reporting-home/widgets/widget-trainings.component';
import { WidgetCalendarComponent } from './components/reporting-home/widgets/widget-calendar.component';
import { PlanningunitService } from './modules/planningunit/planningunit.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import {JwtInterceptor} from './core/helpers/jwt.interceptor';


@NgModule({
    declarations: [
        ReportingComponent,
        ReportingHomeComponent,
        ReportingListComponent,
        ReportingDetailComponent,
        ReportingWidgetsComponent,
        ReportingAlertComponent,
        NavmenuSectionComponent,
        NavmenuGroupComponent,
        NavmenuItemComponent,
        TopNavComponent,
        ProfileListComponent,
        ProfileCurrentComponent,
        ProfileListDetailComponent,
        ReportingProfileEditComponent,
        ReportingHelpHomeComponent,
        WidgetActivitiesAgentComponent,
        WidgetActivitiesProgramAssistantComponent,
        WidgetDDAssistantComponent,
        WidgetDDComponent,
        WidgetMyInfoComponent,
        WidgetSpecialistComponent,
        WidgetStaffAssistantComponent,
        WidgetTrainingsComponent,
        WidgetCalendarComponent
    ],
    imports: [ 
        ReportingRoutingModule,
        SharedModule,
        AdminModule
    ],
    exports:[ReportingProfileEditComponent],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        NavigationService,
        ProfileService,
        PlanningunitService
    ]
})
export class ReportingModule {
}
