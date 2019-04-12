import { User, Image } from '../user/user.service';

export class ExtensionEvent {
    id: number;
    hasAttachments?: boolean;
    subject:string;
    body:string;
    bodyPreview:string;
    start:Date;
    end?:Date;
    location: ExtensionEventLocation;
    locations: ExtensionEventLocation[];
    isAllDay?: boolean;
    isCancelled?:boolean;
    recurrence: ExtensionEventPatternedRecurrence;
    seriesMasterId:string;
    type: ExtensionEventType;
    attendees: User[];
    organizer: User;
    organizerId: number;
    webLink: string;
    onlineMeetingUrl: string;
    extensionEventImages:ExtensionEventImage[];
    createdDateTime:Date;
    lastModifiedDateTime: Date;
    discriminatorValue: string;
}

export class ExtensionEventLocation{
  id:number;
  displayName: string;
  locationEmailAddress: string;
  address:PhysicalAddress;
  coordinates: ExtensionEventGeoCoordinates;
  locationUri: string;
  locationType?: ExtensionEventLocationType;
  uniqueId: string;
  uniqueIdType?: ExtensionEventLocationUniqueIdType;
}

export class PhysicalAddress{
  id: number;
  street: string;
  city: string;
  state: string;
  countryOrRegion: string;
  postalCode: string;
}

export class ExtensionEventGeoCoordinates{
  id: number;
  altitude: number;
  latitude: number;
  longitude: number;
  accuracy?: number;
  AltitudeAccuracy?:number;
}

export class ExtensionEventImage{
  id:number;
  extensionEventId:number;
  extensionEvent: ExtensionEvent;
  uploadImageId:number;
  uploadImage:Image;
  created:Date;
}

export enum ExtensionEventType{
  /// <summary>
  /// single Instance
  /// </summary>
  SingleInstance = 0,

  /// <summary>
  /// occurrence
  /// </summary>
  Occurrence = 1,

  /// <summary>
  /// exception
  /// </summary>
  Exception = 2,

  /// <summary>
  /// series Master
  /// </summary>
  SeriesMaster = 3,
}

export enum ExtensionEventLocationType{
        Default = 0,
	
        /// <summary>
        /// conference Room
        /// </summary>
        ConferenceRoom = 1,
	
        /// <summary>
        /// home Address
        /// </summary>
        HomeAddress = 2,
	
        /// <summary>
        /// business Address
        /// </summary>
        BusinessAddress = 3,
	
        /// <summary>
        /// geo Coordinates
        /// </summary>
        GeoCoordinates = 4,
	
        /// <summary>
        /// street Address
        /// </summary>
        StreetAddress = 5,
	
        /// <summary>
        /// hotel
        /// </summary>
        Hotel = 6,
	
        /// <summary>
        /// restaurant
        /// </summary>
        Restaurant = 7,
	
        /// <summary>
        /// local Business
        /// </summary>
        LocalBusiness = 8,
	
        /// <summary>
        /// postal Address
        /// </summary>
        PostalAddress = 9,

        /// <summary>
        /// extension Office
        /// </summary>
        ExtensionOffice = 10,
}

export enum ExtensionEventLocationUniqueIdType{
    /// <summary>
        /// unknown
        /// </summary>
        Unknown = 0,
	
        /// <summary>
        /// location Store
        /// </summary>
        LocationStore = 1,
	
        /// <summary>
        /// directory
        /// </summary>
        Directory = 2,
	
        /// <summary>
        /// private
        /// </summary>
        Private = 3,
	
        /// <summary>
        /// bing
        /// </summary>
        Bing = 4,

        /// <summary>
        /// bing
        /// </summary>
        PlanningUnit = 5,
}

export class ExtensionEventPatternedRecurrence{
  id:number;
  pattern: ExtensionEventRecurrencePattern;
  range: ExtensionEventRecurrenceRange;
}
export class ExtensionEventRecurrencePattern{
  id:number;
  type?:ExtensionEventRecurrencePatternType;
  interval?:number;
  month?:number;
  dayOfMonth?:number;
  DaysOfWeek:number;
  firstDayOfWeek?: DayOfWeek;
  index?: ExtensionEventWeekIndex;
}
export class ExtensionEventRecurrenceRange{
  id:number;
  type?: ExtensionEventRecurrenceRangeType;
  startDate: Date;
  endDate: Date;
  recurrenceTimeZone: string;
  numberOfOccurrences?:number;
}

export enum ExtensionEventRecurrenceRangeType{
  /// <summary>
  /// end Date
  /// </summary>
  EndDate = 0,

  /// <summary>
  /// no End
  /// </summary>
  NoEnd = 1,

  /// <summary>
  /// numbered
  /// </summary>
  Numbered = 2,
}

export enum ExtensionEventWeekIndex{
  /// <summary>
  /// first
  /// </summary>
  First = 0,

  /// <summary>
  /// second
  /// </summary>
  Second = 1,

  /// <summary>
  /// third
  /// </summary>
  Third = 2,

  /// <summary>
  /// fourth
  /// </summary>
  Fourth = 3,

  /// <summary>
  /// last
  /// </summary>
  Last = 4,
}

export enum ExtensionEventRecurrencePatternType{
    /// <summary>
    /// daily
    /// </summary>
    Daily = 0,

    /// <summary>
    /// weekly
    /// </summary>
    Weekly = 1,

    /// <summary>
    /// absolute Monthly
    /// </summary>
    AbsoluteMonthly = 2,

    /// <summary>
    /// relative Monthly
    /// </summary>
    RelativeMonthly = 3,

    /// <summary>
    /// absolute Yearly
    /// </summary>
    AbsoluteYearly = 4,

    /// <summary>
    /// relative Yearly
    /// </summary>
    RelativeYearly = 5,
}
export enum DayOfWeek{
  //
  // Summary:
  //     Indicates Sunday.
  Sunday = 0,
  //
  // Summary:
  //     Indicates Monday.
  Monday = 1,
  //
  // Summary:
  //     Indicates Tuesday.
  Tuesday = 2,
  //
  // Summary:
  //     Indicates Wednesday.
  Wednesday = 3,
  //
  // Summary:
  //     Indicates Thursday.
  Thursday = 4,
  //
  // Summary:
  //     Indicates Friday.
  Friday = 5,
  //
  // Summary:
  //     Indicates Saturday.
  Saturday = 6
}