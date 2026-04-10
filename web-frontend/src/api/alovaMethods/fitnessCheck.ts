import type { Leaderboard } from '@/models/sportsTest/leaderboard';
import { fitnessCheckAlova } from '../alova';

export const getLeaderboard = () => fitnessCheckAlova.Get<Leaderboard>('leaderboard');
