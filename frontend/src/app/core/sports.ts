export interface SportMeta {
  label: string;
  icon: string;
  color: string;
}

export const SPORTS: Record<string, SportMeta> = {
  running: { label: 'Running', icon: '🏃', color: '#34d399' },
  walking: { label: 'Walking', icon: '🚶', color: '#60a5fa' },
  cycling: { label: 'Cycling', icon: '🚴', color: '#fbbf24' },
  gym: { label: 'Gym', icon: '🏋️', color: '#f472b6' },
  swimming: { label: 'Swimming', icon: '🏊', color: '#22d3ee' },
  dailysteps: { label: 'Daily Steps', icon: '👟', color: '#a78bfa' },
};

export function sportMeta(sport: string): SportMeta {
  return SPORTS[sport.toLowerCase()] ?? { label: sport, icon: '▫️', color: '#94a3b8' };
}
