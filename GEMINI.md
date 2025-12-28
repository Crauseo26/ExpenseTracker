# FinancIA Project GEMINI.md

## Project Overview

FinancIA is a personal finance management application designed to streamline expense tracking. The system is architected as a monorepo with a clear separation of concerns between its core components:

*   **.NET Backend**: The central authority and single source of truth. It handles all domain logic, state management, persistence, and orchestration of AI services. It exposes a REST API for synchronous operations and handles asynchronous jobs for heavy AI workloads.
*   **Python AI Service**: An advisory service responsible for processing inputs (plain text or images via OCR) and proposing structured expense data. It provides confidence scores for its proposals but has no authority over the domain.
*   **Mobile App (Android)**: A thin client for user interaction. It captures expense information (manual entry, text, images), displays the authoritative state from the backend, and allows users to review and confirm AI-generated expense proposals.

The project prioritizes development speed and learning value. It's built with a "code-first, AI-assisted" approach, where the initial structure and guardrails are manually defined, and AI agents are then used for code generation and other development tasks.

## Directory Overview

The project is a "non-code" project at this stage, meaning the directory contains the specifications for a future software project. The code itself has not been written yet.

*   `/specs`: This directory is the **single source of truth** for the project. It contains a series of detailed Markdown files that define the architecture, responsibilities, constraints, and overall design of the FinancIA system. These files are intended to be read-only for AI agents and serve as the blueprint for all development.

## Key Files

The most important files are in the `/specs` directory. They are numbered to be read in order, each building on the last:

*   `00_repository_layout.md`: Defines the monorepo structure, component responsibilities, and rules for AI agent interaction.
*   `01_system_responsibilities.md`: Outlines the clear boundaries and duties of the Mobile App, Backend, and AI Services.
*   `02_constraints.md`: Lists the technical constraints, technology stack choices (.NET, Python, Android), and non-functional requirements for the Minimum Viable Product (MVP).
*   `03_domain_model.md` to `12_master_orchestration_prompt.md`: These files further detail the system's domain, architecture, data models, API contracts, AI pipeline, and agent prompts.

## Building and Running

Since there is no code in the repository yet, there are no build or run commands. The immediate focus is on understanding the specifications within the `/specs` directory to prepare for future code generation.

**TODO**: Add build and run commands here once the backend and AI services are implemented.

## Development Conventions

The development process is guided by the specifications in the `/specs` directory. Key conventions include:

*   **AI-Assisted Development**: The project heavily relies on AI agents for code generation, guided by the detailed specifications and prompts.
*   **Backend Authority**: The .NET backend is the final decision-maker and source of truth. AI services are purely advisory.
*   **Manual Structure Setup**: The initial repository and project skeletons (e.g., `.csproj` files) are created manually. AI agents are only permitted to generate code *within* these existing structures.
*   **Constraint Adherence**: All development must adhere to the constraints outlined in `02_constraints.md`, such as using a relational database and targeting Android for the mobile MVP.
