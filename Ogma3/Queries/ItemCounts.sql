SELECT 'Stories' as name, count(1) as count FROM "Stories" UNION
SELECT 'Chapters' as name, count(1) as count FROM "Chapters" UNION
SELECT 'Blogposts' as name, count(1) as count FROM "Blogposts" UNION
SELECT 'Users' as name, count(1) as count FROM "AspNetUsers" u WHERE u."EmailConfirmed" UNION
SELECT 'Comments' as name, count(1) as count FROM "Comments" c WHERE c."DeletedBy" is null UNION
SELECT 'Reports' as name, count(1) as count FROM "Reports";