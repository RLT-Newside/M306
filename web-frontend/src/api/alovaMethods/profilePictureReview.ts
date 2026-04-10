import {
  Cohort,
  Person,
  ProfilePicture,
  ProfilePictureRequestDetails,
  RequestQueryResponseDTO,
  type ReviewedProfilePicture,
} from '@/models/profilePictureReview/profilePicture';
import { profilePictureAlova } from '../alova';
import type { RejectReason } from '@/models/profilePictureReview/RejectReason';

export const getCohorts = () => {
  return profilePictureAlova.Get<Cohort[]>('cohort');
};

export const getPersons = () => {
  return profilePictureAlova.Get<Person[]>('person');
};

export const getRequests = (
  page: number,
  itemsPerPage: number | null = null,
  search = '',
  state = ''
) => {
  const params: Record<string, number | string> = {
    page: page,
  };

  if (itemsPerPage != null) {
    params['count'] = itemsPerPage;
  }

  if (search.length >= 2) {
    params['search'] = search;
  }

  if (state) {
    params['state'] = state;
  }

  return profilePictureAlova.Get<RequestQueryResponseDTO>('request', { params: params });
};

export const getRequestDetails = (requestId: string) => {
  return profilePictureAlova.Get<ProfilePictureRequestDetails>(`request/${requestId}`);
};

export interface TokenValidationResponse {
  token: string;
  requestId: string;
  requestState: number;
  requestExpiry: string;
  firstName?: string;
  lastName?: string;
  cohortUniqueName?: string;
  cohortDisplayName?: string;
  statusCode?: number;
  statusMessage?: string;
}

export const validateRequestToken = (token: string) => {
  return profilePictureAlova.Get<TokenValidationResponse>(`Token`, {
    params: { verify: token },
  });
};

export const sendReminder = (requestId: string) => {
  return profilePictureAlova.Post(`request/${requestId}/reminder`);
};

export const getRejectReasons = () => {
  return profilePictureAlova.Get<RejectReason[]>('rejectReason');
};

export const getPendingReviews = () =>
  profilePictureAlova.Get<ProfilePicture[]>('review', {
    params: {
      take: 24,
    },
    transform(data: unknown) {
      const profilePictures: ProfilePicture[] = [];
      (data as Array<Record<string, unknown>>).forEach((image) => {
        profilePictures.push(
          new ProfilePicture(
            image.id as string,
            image.url as string,
            image.uploadedAt as string,
            image.subjectFirstName as string,
            image.subjectLastName as string,
            image.cohortDisplayName as string,
            image.cohortUniqueName as string
          )
        );
      });
      return profilePictures;
    },
  });

export const reviewProfilePictures = (reviewedProfilePictures: ReviewedProfilePicture[]) =>
  profilePictureAlova.Post<ReviewedProfilePicture[]>(
    'review',
    JSON.stringify(reviewedProfilePictures)
  );
