maze = []
with open('str_test.txt') as f:
    for line in f:
        line = line.replace("'","")
        line = line.replace("[", "")
        line = line.replace("]", "")
        line = line.replace("\n", "")
        maze.append(line.split(','))

#print(maze)

maze_list = []

for i in range(1, len(maze)-1,2):
    for j in range(1, len(maze[0])-1,2):

        # Check northern wall
        if maze[i-1][j] ==' O':
            maze_list.append([i,j,0])
        # Check eastern wall
        if maze[i][j+1]==' O':
            maze_list.append([i,j,1])
        #check western wall
        if maze[i][j-1]==' O':
            maze_list.append([i,j,3])
        #check southern wall
        if maze[i+1][j]==' O':
            maze_list.append([i,j,2])

print(maze_list)

# thefile = open('maze_list_file.txt', 'w')
# 
# for item in maze_list:
#     thefile.write(item)

with open("maze_list_text.txt", "w") as file:
    file.write(str(maze_list))
