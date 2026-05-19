WITH days AS (SELECT GENERATE_SERIES(CURRENT_DATE - INTERVAL '13 days', CURRENT_DATE, INTERVAL '1 day')::date AS day),
     counts AS (SELECT 'Users' AS name, d.day, COUNT(u."Id") AS cnt
                FROM days d
						 LEFT JOIN "AspNetUsers" u
								   ON u."RegistrationDate" >= d.day::timestamptz
									   AND u."RegistrationDate" < (d.day + 1)::timestamptz
									   AND u."EmailConfirmed" = TRUE
                GROUP BY d.day

                UNION ALL

                SELECT 'Stories', d.day, COUNT(s."Id")
                FROM days d
						 LEFT JOIN "Stories" s
								   ON s."CreationDate" >= d.day::timestamptz
									   AND s."CreationDate" < (d.day + 1)::timestamptz
                GROUP BY d.day

                UNION ALL

                SELECT 'Blogposts', d.day, COUNT(b."Id")
                FROM days d
						 LEFT JOIN "Blogposts" b
								   ON b."CreationDate" >= d.day::timestamptz
									   AND b."CreationDate" < (d.day + 1)::timestamptz
                GROUP BY d.day

                UNION ALL

                SELECT 'Comments', d.day, COUNT(c."Id")
                FROM days d
						 LEFT JOIN "Comments" c
								   ON c."DateTime" >= d.day::timestamptz
									   AND c."DateTime" < (d.day + 1)::timestamptz
									   AND c."DeletedBy" IS NULL
                GROUP BY d.day

                UNION ALL

                SELECT 'Chapters', d.day, COUNT(ch."Id")
                FROM days d
						 LEFT JOIN "Chapters" ch
								   ON ch."CreationDate" >= d.day::timestamptz
									   AND ch."CreationDate" < (d.day + 1)::timestamptz
                GROUP BY d.day

                UNION ALL

                SELECT 'Reports', d.day, COUNT(r."Id")
                FROM days d
						 LEFT JOIN "Reports" r
								   ON r."ReportDate" >= d.day::timestamptz
									   AND r."ReportDate" < (d.day + 1)::timestamptz
									   AND r."Status" = 'open'::report_status
                GROUP BY d.day)

SELECT name, ARRAY_AGG(cnt ORDER BY day) AS data
FROM counts
GROUP BY name;