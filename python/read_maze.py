maze = []
with open('str_test.txt') as f:
	for line in f:
		line = line.replace("'","")
		line = line.replace("[", "")
		line = line.replace("]", "")
		line = line.replace("\n", "")
		maze.append(line.split(','))

print(maze)

maze_list = []

for i in range(1, len(maze)-1,2):
	for j in range(1, len(maze[0])-1,2):

		# Check northern wall 
		print(0)
		if maze[i-1][j] ==' O':
			maze_list.append([i,j,0,1])
		# Check east wall
		if maze[i][j+1]==' O':
			maze_list.append([i,j,1,1])

print(maze_list)


