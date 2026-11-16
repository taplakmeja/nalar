# Phase 4 Plan — Training & Examination System

## Scope
Server-side session lifecycle for all six training modes, server-side scoring,
examination profiles, and the mobile-first training/exam UI.

## Tasks
- Domain: `TrainingSession`, `SessionQuestion` (generation metadata + answer key
  only — no rendered payload), `ExamProfile`, `TrainingMode` enum.
- Application: StartSession, GetSessionQuestion (regenerate from metadata),
  SubmitAnswer (server-side validation + concise feedback), FinishSession,
  GetSessionResult; `IScoringService` (simple, weighted, exam-specific).
- Infrastructure: 6 seeded exam profiles; migrations.
- Web: Sessions + ExamProfiles endpoints (auth-required).
- Frontend: practice flow with immediate feedback; exam interface (countdown,
  question navigation, review marking, auto-submit, distraction-free); result page.

## Acceptance Criteria
1. Answers are validated and scored server-side only; correct answers never
   sent to the client before submission.
2. Session questions are reproducible from stored metadata (no payload storage).
3. All six training modes startable; exam simulation enforces time limit.
4. Six exam profiles seeded and selectable.
5. Integration tests: start → answer → finish → result; scoring correct.
6. Frontend tests green for the practice flow guards/feedback.
