import {Injectable} from "@angular/core";
declare var ga:Function;

@Injectable()
export class GoogleAnalyticsEventsService {
 

  /*

sample from: https://blog.thecodecampus.de/angular-2-include-google-analytics-event-tracking/

*****************

submitEvent() {
    this.googleAnalyticsEventsService.emitEvent("testCategory", "testAction", "testLabel", 10);
  }

  */


  public emitEvent(eventCategory: string,
                   eventAction: string,
                   eventLabel: string = null,
                   eventValue: number = null) {
    ga('send', 'event', {
      eventCategory: eventCategory,
      eventLabel: eventLabel,
      eventAction: eventAction,
      eventValue: eventValue
    });
  }
}