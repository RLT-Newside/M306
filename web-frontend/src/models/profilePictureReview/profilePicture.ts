import type { RejectReason } from './RejectReason';
import type { RequestState } from './RequestState';
import { ReviewState } from './ReviewState';
import type { ValidationState } from './ValidationState';

export class Cohort {
  id: string;
  uniqueName: string;
  displayName: string;

  constructor(id: string, uniqueName: string, displayName: string) {
    this.id = id;
    this.uniqueName = uniqueName;
    this.displayName = displayName;
  }
}

export class PersonSearchResult {
  gib: string;
  userName: string;
  firstName: string;
  lastName: string;
  classUniqueName: string;
  classDisplayName: string;

  constructor(
    gib: string,
    userName: string,
    firstName: string,
    lastName: string,
    classUniqueName: string,
    classDisplayName: string
  ) {
    this.gib = gib;
    this.userName = userName;
    this.firstName = firstName;
    this.lastName = lastName;
    this.classUniqueName = classUniqueName;
    this.classDisplayName = classDisplayName;
  }
}

export class Person {
  id: string;
  publicIdentifier: string;
  userName: string;
  firstName: string;
  lastName: string;
  cohortId: string;

  constructor(
    id: string,
    publicIdentifier: string,
    userName: string,
    firstName: string,
    lastName: string,
    cohortId: string
  ) {
    this.id = id;
    this.publicIdentifier = publicIdentifier;
    this.userName = userName;
    this.firstName = firstName;
    this.lastName = lastName;
    this.cohortId = cohortId;
  }
}

export class RequestQueryResponseDTO {
  total: number;
  requests: ProfilePictureRequest[];

  constructor(total: number, requests: ProfilePictureRequest[]) {
    this.total = total;
    this.requests = requests;
  }
}

export class ProfilePictureRequest {
  id: string;
  token: string;
  personId: string;
  requestState: RequestState;
  validationState: ValidationState;
  expiryDate: string;
  previousRequestId: string;
  requestSequenceIndex: string;

  constructor(
    id: string,
    token: string,
    personId: string,
    requestState: RequestState,
    validationState: ValidationState,
    expiryDate: string,
    previousRequestId: string,
    requestSequenceIndex: string
  ) {
    this.id = id;
    this.token = token;
    this.personId = personId;
    this.requestState = requestState;
    this.validationState = validationState;
    this.expiryDate = expiryDate;
    this.previousRequestId = previousRequestId;
    this.requestSequenceIndex = requestSequenceIndex;
  }
}

export class ProfilePictureRequestWithPerson extends ProfilePictureRequest {
  person: Person;
  cohort: Cohort;

  constructor(
    id: string,
    token: string,
    personId: string,
    requestState: RequestState,
    validationState: ValidationState,
    expiryDate: string,
    previousRequestId: string,
    requestSequenceIndex: string,
    person: Person,
    cohort: Cohort
  ) {
    super(
      id,
      token,
      personId,
      requestState,
      validationState,
      expiryDate,
      previousRequestId,
      requestSequenceIndex
    );
    this.person = person;
    this.cohort = cohort;
  }
}

export class ProfilePictureRequestDetails {
  request: ProfilePictureRequestWithPerson;
  profilePicture: ProfilePictureRequestProfilePicture;
  profilePictureReview: ProfilePictureReview;
  rejectReason: RejectReason;

  constructor(
    request: ProfilePictureRequestWithPerson,
    profilePicture: ProfilePictureRequestProfilePicture,
    profilePictureReview: ProfilePictureReview,
    rejectReason: RejectReason
  ) {
    this.request = request;
    this.profilePicture = profilePicture;
    this.profilePictureReview = profilePictureReview;
    this.rejectReason = rejectReason;
  }
}

export class ProfilePictureRequestProfilePicture {
  id: string;
  personId: string;
  fileName: string;
  uploadDateTime: string;
  profilePictureRequestId: string;
  downloadURL: string;

  constructor(
    id: string,
    personId: string,
    fileName: string,
    uploadDateTime: string,
    profilePictureRequestId: string,
    downloadURL: string
  ) {
    this.id = id;
    this.personId = personId;
    this.fileName = fileName;
    this.uploadDateTime = uploadDateTime;
    this.profilePictureRequestId = profilePictureRequestId;
    this.downloadURL = downloadURL;
  }
}

export class ProfilePictureReview {
  id: string;
  reviewerId: string;
  reviewerUserName: string;
  profilePictureId: string;
  reviewState: number;
  reviewDateTime: string;
  rejectReasonId: string;

  constructor(
    id: string,
    reviewerId: string,
    reviewerUserName: string,
    profilePictureId: string,
    reviewState: number,
    reviewDateTime: string,
    rejectReasonId: string
  ) {
    this.id = id;
    this.reviewerId = reviewerId;
    this.reviewerUserName = reviewerUserName;
    this.profilePictureId = profilePictureId;
    this.reviewState = reviewState;
    this.reviewDateTime = reviewDateTime;
    this.rejectReasonId = rejectReasonId;
  }
}

export class ProfilePicture {
  id: string;
  url: string;
  uploadedAt: Date;
  subjectFirstName: string;
  subjectLastName: string;
  cohortDisplayName: string;
  cohortUniqueName: string;
  status: ReviewState;
  rejectReason: RejectReason | null = null;

  constructor(
    id: string,
    url: string,
    uploadedAt: string,
    subjectFirstName: string,
    subjectLastName: string,
    cohortDisplayName: string,
    cohortUniqueName: string
  ) {
    this.id = id;
    this.url = url;
    this.uploadedAt = new Date(uploadedAt);
    this.subjectFirstName = subjectFirstName;
    this.subjectLastName = subjectLastName;
    this.cohortDisplayName = cohortDisplayName;
    this.cohortUniqueName = cohortUniqueName;
    this.status = ReviewState.Pending;
  }
}

export class ReviewedProfilePicture {
  profilePictureId: string;
  reviewState: ReviewState;
  rejectReasonId: string | null;

  constructor(profilePictureId: string, reviewState: ReviewState, rejectReasonId: string | null) {
    this.profilePictureId = profilePictureId;
    this.reviewState = reviewState;
    this.rejectReasonId = rejectReasonId;
  }
}
