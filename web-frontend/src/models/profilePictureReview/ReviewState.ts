export enum ReviewState {
  Pending,
  InProgress,
  Accepted,
  Rejected,
}

export function getReviewStateLabel(state: ReviewState | string): string {
  console.log(state, typeof state);
  switch (state) {
    case ReviewState.Pending:
    case 'Pending':
      return 'pendent';
    case ReviewState.InProgress:
    case 'InProgress':
      return 'Review läuft';
    case ReviewState.Accepted:
    case 'Accepted':
      return 'Portraitfoto akzeptiert';
    case ReviewState.Rejected:
    case 'Rejected':
      return 'Portraitfoto abgelehnt';
    default:
      return 'unbekannt';
  }
}
