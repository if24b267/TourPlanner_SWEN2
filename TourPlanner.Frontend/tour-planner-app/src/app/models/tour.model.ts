export interface Tour {
  id?: string;
  name: string;
  tourDescription: string;
  from: string;
  to: string;
  transportType: string;
  tourDistance: number;
  estimatedTimeHours: number;
  routeImagePath?: string;
  tourLogs: TourLog[];
  popularity: number;
  childFriendliness?: number;
}

export interface TourLog {
  id?: string;
  dateTime: Date;
  comment: string;
  difficulty: number;
  totalDistance: number;
  totalTimeHours: number;
  rating: number;
}