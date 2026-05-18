SELECT q."Author", q."Body"
FROM "Quotes" q
	     TABLESAMPLE SYSTEM_ROWS(1)
LIMIT 1