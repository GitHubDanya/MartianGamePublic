--
-- PostgreSQL database dump
--

\restrict r21jMfqyN13T4Go20TFbBWlc0lDioULTchHMtri38mELjc1QkstNd4C2BUD2TLF

-- Dumped from database version 15.16 (Debian 15.16-0+deb12u1)
-- Dumped by pg_dump version 15.16 (Debian 15.16-0+deb12u1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: spanswers_add(character varying, text, timestamp without time zone, character varying, character varying, double precision, character varying, integer, double precision, double precision, double precision); Type: PROCEDURE; Schema: public; Owner: www-data
--

CREATE PROCEDURE public.spanswers_add(IN p_userid character varying, IN p_answer text, IN p_submittime timestamp without time zone, IN p_solutioncategory character varying, IN p_coverage character varying, IN p_coveragescore double precision, IN p_answerkind character varying, IN p_triesleft integer, IN p_totalscore double precision, IN p_dsi double precision, IN p_distancefromoptimal double precision)
    LANGUAGE plpgsql
    AS $$
BEGIN

	INSERT INTO answers (userid, answer, submittime, solutioncategory, coverage, coveragescore, answerkind, triesleft, totalscore, dsi, distancefromoptimal)
	VALUES (p_userid, p_answer, p_submittime, p_solutioncategory, p_coverage, p_coveragescore, p_answerkind, p_triesleft, p_totalscore, p_dsi, p_distancefromoptimal);

END;
$$;


ALTER PROCEDURE public.spanswers_add(IN p_userid character varying, IN p_answer text, IN p_submittime timestamp without time zone, IN p_solutioncategory character varying, IN p_coverage character varying, IN p_coveragescore double precision, IN p_answerkind character varying, IN p_triesleft integer, IN p_totalscore double precision, IN p_dsi double precision, IN p_distancefromoptimal double precision) OWNER TO "www-data";

--
-- Name: spconversations_add(character varying, timestamp without time zone, text, text, character varying, integer, character varying, integer, double precision, integer, double precision, double precision, integer); Type: PROCEDURE; Schema: public; Owner: www-data
--

CREATE PROCEDURE public.spconversations_add(IN p_userid character varying, IN p_requesttime timestamp without time zone, IN p_request text, IN p_response text, IN p_complexitylevel character varying, IN p_complexitylevelscore integer, IN p_questiontype character varying, IN p_questiontypescore integer, IN p_relevance double precision, IN p_consistency integer, IN p_representativeness double precision, IN p_totalscore double precision, IN p_user_entry_order integer DEFAULT NULL::integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
	INSERT INTO conversations (
		userid, requesttime, request, response,
		complexitylevel, complexitylevelscore,
		questiontype, questiontypescore,
		relevance, consistency, representativeness, totalscore
	)
	VALUES (
		p_userid, p_requesttime, p_request, p_response,
		p_complexitylevel, p_complexitylevelscore,
		p_questiontype, p_questiontypescore,
		p_relevance, p_consistency, p_representativeness, p_totalscore
	);

	WITH ranked AS (
		SELECT
			id,
			ROW_NUMBER() OVER (PARTITION BY userid ORDER BY requesttime ASC) AS rn
		FROM conversations
		WHERE userid = p_userid
	)
	UPDATE conversations
	SET user_entry_order = ranked.rn
	FROM ranked
	WHERE conversations.id = ranked.id;
END;
$$;


ALTER PROCEDURE public.spconversations_add(IN p_userid character varying, IN p_requesttime timestamp without time zone, IN p_request text, IN p_response text, IN p_complexitylevel character varying, IN p_complexitylevelscore integer, IN p_questiontype character varying, IN p_questiontypescore integer, IN p_relevance double precision, IN p_consistency integer, IN p_representativeness double precision, IN p_totalscore double precision, IN p_user_entry_order integer) OWNER TO "www-data";

--
-- Name: sppersonalinfo_add(text, text, integer, text, boolean, text, text); Type: PROCEDURE; Schema: public; Owner: www-data
--

CREATE PROCEDURE public.sppersonalinfo_add(IN p_userid text, IN p_experimentid text, IN p_age integer, IN p_gender text, IN p_isenglishfirstlanguage boolean, IN p_country text, IN p_education text)
    LANGUAGE plpgsql
    AS $$
BEGIN

	INSERT INTO personalinfo (userid, experimentid, age, gender, isenglishfirstlanguage, country, education)
	VALUES (p_userid, p_experimentid, p_age, p_gender, p_isenglishfirstlanguage, p_country, p_education);

END;
$$;


ALTER PROCEDURE public.sppersonalinfo_add(IN p_userid text, IN p_experimentid text, IN p_age integer, IN p_gender text, IN p_isenglishfirstlanguage boolean, IN p_country text, IN p_education text) OWNER TO "www-data";

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: answers; Type: TABLE; Schema: public; Owner: www-data
--

CREATE TABLE public.answers (
    id integer NOT NULL,
    userid character varying(100) NOT NULL,
    answer text NOT NULL,
    submittime timestamp without time zone NOT NULL,
    solutioncategory character varying(30) NOT NULL,
    coverage character varying(50) NOT NULL,
    coveragescore double precision NOT NULL,
    answerkind character varying(30) NOT NULL,
    triesleft integer NOT NULL,
    totalscore double precision,
    dsi double precision,
    distancefromoptimal double precision
);


ALTER TABLE public.answers OWNER TO "www-data";

--
-- Name: answers_id_seq; Type: SEQUENCE; Schema: public; Owner: www-data
--

CREATE SEQUENCE public.answers_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.answers_id_seq OWNER TO "www-data";

--
-- Name: answers_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: www-data
--

ALTER SEQUENCE public.answers_id_seq OWNED BY public.answers.id;


--
-- Name: conversations; Type: TABLE; Schema: public; Owner: www-data
--

CREATE TABLE public.conversations (
    id integer NOT NULL,
    userid character varying(100) NOT NULL,
    requesttime timestamp without time zone NOT NULL,
    request text NOT NULL,
    response text NOT NULL,
    complexitylevel character varying(32) NOT NULL,
    complexitylevelscore integer NOT NULL,
    questiontype character varying(32) NOT NULL,
    questiontypescore integer NOT NULL,
    relevance double precision NOT NULL,
    consistency integer NOT NULL,
    representativeness double precision NOT NULL,
    totalscore double precision NOT NULL,
    user_entry_order integer
);


ALTER TABLE public.conversations OWNER TO "www-data";

--
-- Name: conversations_id_seq; Type: SEQUENCE; Schema: public; Owner: www-data
--

CREATE SEQUENCE public.conversations_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.conversations_id_seq OWNER TO "www-data";

--
-- Name: conversations_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: www-data
--

ALTER SEQUENCE public.conversations_id_seq OWNED BY public.conversations.id;


--
-- Name: personalinfo; Type: TABLE; Schema: public; Owner: www-data
--

CREATE TABLE public.personalinfo (
    id integer NOT NULL,
    userid text NOT NULL,
    experimentid text NOT NULL,
    age integer,
    gender text,
    isenglishfirstlanguage boolean,
    country text,
    education text
);


ALTER TABLE public.personalinfo OWNER TO "www-data";

--
-- Name: personalinfo_id_seq; Type: SEQUENCE; Schema: public; Owner: www-data
--

CREATE SEQUENCE public.personalinfo_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.personalinfo_id_seq OWNER TO "www-data";

--
-- Name: personalinfo_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: www-data
--

ALTER SEQUENCE public.personalinfo_id_seq OWNED BY public.personalinfo.id;


--
-- Name: answers id; Type: DEFAULT; Schema: public; Owner: www-data
--

ALTER TABLE ONLY public.answers ALTER COLUMN id SET DEFAULT nextval('public.answers_id_seq'::regclass);


--
-- Name: conversations id; Type: DEFAULT; Schema: public; Owner: www-data
--

ALTER TABLE ONLY public.conversations ALTER COLUMN id SET DEFAULT nextval('public.conversations_id_seq'::regclass);


--
-- Name: personalinfo id; Type: DEFAULT; Schema: public; Owner: www-data
--

ALTER TABLE ONLY public.personalinfo ALTER COLUMN id SET DEFAULT nextval('public.personalinfo_id_seq'::regclass);


--
-- Name: answers answers_pkey; Type: CONSTRAINT; Schema: public; Owner: www-data
--

ALTER TABLE ONLY public.answers
    ADD CONSTRAINT answers_pkey PRIMARY KEY (id);


--
-- Name: conversations conversations_pkey; Type: CONSTRAINT; Schema: public; Owner: www-data
--

ALTER TABLE ONLY public.conversations
    ADD CONSTRAINT conversations_pkey PRIMARY KEY (id);


--
-- Name: personalinfo personalinfo_pkey; Type: CONSTRAINT; Schema: public; Owner: www-data
--

ALTER TABLE ONLY public.personalinfo
    ADD CONSTRAINT personalinfo_pkey PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

\unrestrict r21jMfqyN13T4Go20TFbBWlc0lDioULTchHMtri38mELjc1QkstNd4C2BUD2TLF

