// Compatibility types for the retired 'angular-mydatepicker' / 'mydatepicker' packages.

export interface IMyDate {
  year: number;
  month: number;
  day: number;
}

export interface IMySingleDateModel {
  jsDate: Date;
  date?: IMyDate;
  formatted?: string;
  epoc?: number;
}

export interface IMyDateRangeModel {
  beginDate: IMyDate;
  endDate: IMyDate;
  beginJsDate?: Date;
  endJsDate?: Date;
  formatted?: string;
}

// Unified event/model shape covering both legacy libraries' consumption patterns
// (event.singleDate.jsDate, event.dateRange.beginJsDate, event.date.year, event.jsdate).
export interface IMyDateModel {
  isRange: boolean;
  singleDate: IMySingleDateModel | null;
  dateRange: IMyDateRangeModel | null;
  date?: IMyDate;
  jsdate?: Date;
  formatted?: string;
}

export interface IAngularMyDpOptions {
  dateFormat?: string;
  dateRange?: boolean;
  satHighlight?: boolean;
  firstDayOfWeek?: string;
  showTodayBtn?: boolean;
  showFooterToday?: boolean;
  alignSelectorRight?: boolean;
  disableSince?: IMyDate;
  disableUntil?: IMyDate;
  editableDateField?: boolean;
  showClearDateBtn?: boolean;
}

// Alias kept for code that imported from the older 'mydatepicker' package.
export type IMyDpOptions = IAngularMyDpOptions;
