"""
Script to resolve merge conflicts in WarehouseDbContext.cs
Strategy: For each conflict, take the 'origin/main' side (right side)
except for the first conflict where we set up proper using statements
"""

import re

filepath = r"c:\Users\asus\source\repos\Group7-Martirial-Managements\G7-Asigments\DAL.DataAccessLayer\Context\WarehouseDbContext.cs"

with open(filepath, 'r', encoding='utf-8-sig') as f:
    content = f.read()

print(f"File size: {len(content)} bytes")

# Pattern to match conflict blocks
CONFLICT_PATTERN = re.compile(
    r'<<<<<<< HEAD\r?\n(.*?)\r?\n=======\r?\n(.*?)\r?\n>>>>>>> origin/main',
    re.DOTALL
)

conflicts = CONFLICT_PATTERN.findall(content)
print(f"Found {len(conflicts)} conflicts")

# Show first few
for i, (head, main) in enumerate(conflicts[:5]):
    print(f"\nConflict {i+1}:")
    print(f"  HEAD ({len(head)} chars): {repr(head[:120])}")
    print(f"  MAIN ({len(main)} chars): {repr(main[:120])}")
