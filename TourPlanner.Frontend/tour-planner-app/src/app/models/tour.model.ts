// Muss mit den ORS-Profilen in OpenRouteService.cs (Backend) uebereinstimmen
export const TRANSPORT_TYPES = ['Car', 'Bike', 'Hike', 'Running', 'Vacation'];

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
  routeGeometryJson?: string;
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

export function createBlankTour(): Tour {
  return {
    name: '',
    tourDescription: '',
    from: '',
    to: '',
    transportType: 'Car',
    tourDistance: 0,
    estimatedTimeHours: 0,
    popularity: 0,
    tourLogs: []
  };
}