export type SportKey = 'running' | 'walking' | 'cycling' | 'gym' | 'swimming';

export interface UserSummary {
  id: string;
  firstName: string;
  lastName: string;
}

export interface RegisterUserResponse {
  id: string;
  firstName: string;
  lastName: string;
}

export type Trend = 'up' | 'down' | 'same' | 'new';

export interface LeaderboardEntry {
  rank: number;
  userId: string;
  firstName: string;
  lastName: string;
  totalPoints: number;
  previousRank: number | null;
  rankDelta: number;
  trend: Trend;
}

export interface ActivityHistoryItem {
  id: string;
  sport: string;
  occurredAt: string;
  points: number;
  distanceKm: number | null;
  duration: string | null;
  steps: number | null;
}

export interface DailyVolumePoint {
  date: string;
  points: number;
  activityCount: number;
}

export interface SportBreakdownSlice {
  sport: string;
  points: number;
  activityCount: number;
}

export interface Dashboard {
  userId: string;
  firstName: string;
  lastName: string;
  totalPoints: number;
  rank: number | null;
  activityCount: number;
  currentStreak: number;
  longestStreak: number;
  history: ActivityHistoryItem[];
  volumeOverTime: DailyVolumePoint[];
  sportBreakdown: SportBreakdownSlice[];
}

export interface IngestActivityRequest {
  userId: string;
  datetime: string;
  sport?: SportKey;
  distance?: number;
  duration?: string;
  steps?: number;
}

export interface IngestActivityResponse {
  id: string;
  userId: string;
  sport: string;
  occurredAt: string;
  points: number;
}
