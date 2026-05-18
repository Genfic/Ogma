WITH days AS (SELECT generate_series(
		                     CURRENT_DATE - INTERVAL '15 days',
		                     CURRENT_DATE,
		                     INTERVAL '1 day'
                     )::date AS day),
     counts AS (SELECT 'Users' AS name, d.day, COUNT(u."Id") AS cnt
                FROM days d
	                     LEFT JOIN "AspNetUsers" u ON u."RegistrationDate" >= d.day::timestamptz
	                AND u."RegistrationDate" < (d.day + 1)::timestamptz
                WHERE u."EmailConfirmed"
                GROUP BY d.day

                UNION ALL

                SELECT 'Stories', d.day, COUNT(s."Id")
                FROM days d
	                     LEFT JOIN "Stories" s ON s."CreationDate" >= d.day::timestamptz
	                AND s."CreationDate" < (d.day + 1)::timestamptz
                GROUP BY d.day

                UNION ALL

                SELECT 'Blogposts', d.day, COUNT(b."Id")
                FROM days d
	                     LEFT JOIN "Blogposts" b ON b."CreationDate" >= d.day::timestamptz
	                AND b."CreationDate" < (d.day + 1)::timestamptz
                GROUP BY d.day

                UNION ALL

                SELECT 'Comments', d.day, COUNT(c."Id")
                FROM days d
	                     LEFT JOIN "Comments" c ON c."DateTime" >= d.day::timestamptz
	                AND c."DateTime" < (d.day + 1)::timestamptz
                WHERE c."DeletedBy" is null
                GROUP BY d.day

                UNION ALL

                SELECT 'Chapters', d.day, COUNT(ch."Id")
                FROM days d
	                     LEFT JOIN "Chapters" ch ON ch."CreationDate" >= d.day::timestamptz
	                AND ch."CreationDate" < (d.day + 1)::timestamptz
                GROUP BY d.day)
SELECT name, array_agg(cnt ORDER BY day) AS data
FROM counts
GROUP BY name;