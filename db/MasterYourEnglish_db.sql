CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,                      
    first_name VARCHAR(50) NOT NULL,                
    last_name VARCHAR(50) NOT NULL,                  
    user_name VARCHAR(50) UNIQUE NOT NULL,           
    email VARCHAR(100) UNIQUE NOT NULL,              
    password_hash VARCHAR(255) NOT NULL,             
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  
    role VARCHAR(20) CHECK (role IN ('user', 'admin')) DEFAULT 'user'  
);

CREATE TABLE topics (
    topic_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(255)
);

CREATE TABLE flashcards (
    flashcard_id SERIAL PRIMARY KEY,
    word VARCHAR(100) NOT NULL,
    difficulty_level VARCHAR(5) CHECK (difficulty_level IN ('A1', 'A2', 'B1', 'B2', 'C1', 'C2')),
    transcription VARCHAR(50),
    meaning VARCHAR(255),
    part_of_speech VARCHAR(50),
    example VARCHAR(255),
	UNIQUE (word, topic_id),
    topic_id INTEGER REFERENCES topics(topic_id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    is_user_created BOOLEAN DEFAULT FALSE
);

CREATE TABLE flashcards_bundles (
    flashcards_bundle_id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description VARCHAR(255),
    difficulty_level VARCHAR(5) CHECK (difficulty_level IN ('A1','A2','B1','B2','C1','C2')),
    topic_id INTEGER REFERENCES topics(topic_id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    is_published BOOLEAN DEFAULT FALSE,
    is_user_created BOOLEAN DEFAULT FALSE,
    total_flashcards_count INTEGER DEFAULT 0
);

CREATE TABLE flashcards_bundle_items (
    flashcards_bundle_id INTEGER REFERENCES flashcards_bundles(flashcards_bundle_id) ON DELETE CASCADE,
    flashcard_id INTEGER REFERENCES flashcards(flashcard_id) ON DELETE CASCADE,
    position INTEGER DEFAULT 1,
    PRIMARY KEY (flashcards_bundle_id, flashcard_id)
);

CREATE TABLE saved_flashcards (
    user_id INTEGER REFERENCES users(user_id) ON DELETE CASCADE,
    flashcard_id INTEGER REFERENCES flashcards(flashcard_id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, flashcard_id)
);

CREATE TABLE flashcards_bundle_attempts (
    attempt_id SERIAL PRIMARY KEY,
    flashcards_bundle_id INTEGER REFERENCES flashcards_bundles(flashcards_bundle_id) ON DELETE CASCADE,
    user_id INTEGER REFERENCES users(user_id) ON DELETE CASCADE,
    started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    finished_at TIMESTAMP DEFAULT NULL
);

CREATE TABLE flashcards_attempts_answers (
    answer_id SERIAL PRIMARY KEY,
    attempt_id INTEGER REFERENCES flashcards_bundle_attempts(attempt_id) ON DELETE CASCADE,
    flashcard_id INTEGER REFERENCES flashcards(flashcard_id) ON DELETE CASCADE,
    is_known BOOLEAN DEFAULT FALSE
);

CREATE TABLE tests (
    test_id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description VARCHAR(500),
    topic_id INTEGER REFERENCES topics(topic_id) ON DELETE SET NULL,
    difficulty_level VARCHAR(5) CHECK (difficulty_level IN ('A1','A2','B1','B2','C1','C2')),
    created_by INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_published BOOLEAN DEFAULT FALSE,
    is_user_created BOOLEAN DEFAULT FALSE,
    total_questions_count INT DEFAULT 0
);

CREATE TABLE questions (
    question_id SERIAL PRIMARY KEY,
    text VARCHAR(500) NOT NULL,
    question_type VARCHAR(20) CHECK (question_type IN ('single_choice', 'multiple_choice', 'text_input')),
    difficulty_level VARCHAR(5) CHECK (difficulty_level IN ('A1','A2','B1','B2','C1','C2')),
    created_by INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    topic_id INTEGER REFERENCES topics(topic_id) ON DELETE SET NULL
);

CREATE TABLE question_options (
    option_id SERIAL PRIMARY KEY,
    question_id INTEGER REFERENCES questions(question_id) ON DELETE CASCADE,
    text VARCHAR(255) NOT NULL,
    is_correct BOOLEAN DEFAULT FALSE
);

CREATE TABLE test_questions (
    test_id INTEGER REFERENCES tests(test_id) ON DELETE CASCADE,
    question_id INTEGER REFERENCES questions(question_id) ON DELETE CASCADE,
    position INTEGER DEFAULT 1,
    PRIMARY KEY (test_id, question_id)
);

CREATE TABLE test_attempts (
    attempt_id SERIAL PRIMARY KEY,
    test_id INTEGER REFERENCES tests(test_id) ON DELETE CASCADE,
    user_id INTEGER REFERENCES users(user_id) ON DELETE CASCADE,
    started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    finished_at TIMESTAMP,
    score FLOAT DEFAULT 0
);

CREATE TABLE test_attempts_answers (
    attempt_answers_id SERIAL PRIMARY KEY,
    attempt_id INTEGER REFERENCES test_attempts(attempt_id) ON DELETE CASCADE,
    question_id INTEGER REFERENCES questions(question_id) ON DELETE CASCADE,
    user_answer_text VARCHAR(500),
    selected_option_id INTEGER REFERENCES question_options(option_id) ON DELETE SET NULL,
    is_correct BOOLEAN DEFAULT FALSE
);

CREATE TABLE test_attempts_answers_selected_options (
    attempt_answers_id INTEGER REFERENCES test_attempts_answers(attempt_answers_id) ON DELETE CASCADE,
    selected_option_id INTEGER REFERENCES question_options(option_id) ON DELETE CASCADE,
    PRIMARY KEY (attempt_answers_id, selected_option_id)
);

