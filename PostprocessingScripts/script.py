# 
# Postprocessing Script
#

import csv

results = []
maxes = []
mp = {}

with open('../Results/RAW-11-05-12-37-45.txt') as csvFile:
	reader = csv.reader(csvFile)
	for row in reader:
		questionId = float(row[0])
		answerId = float(row[1])
		oracleResults = [float(x) for x in row[2:]]

		if questionId not in mp:
			mp[questionId] = {}

		mp[questionId][answerId] = oracleResults

		# Now, do some processing on the results...
		for result in oracleResults:
			results.append(result)

		maxes.append(max(oracleResults))

# # Now, get the average
# print "Average: " + str(sum(results)/len(results))
# print "Average Max: " + str(sum(maxes)/len(maxes))

# thres = 0.00
# for i in xrange(0, 20):
# 	thres += 0.05
# 	# Results 
# 	res = [x for x in results if x > thres]

# 	print "Number of results for thres:%s: %d" % (thres, len(res))  

# 	numQAnswered = 0
# 	numQAnsweredMultiple = 0
# 	for question in mp.keys():
# 		ans = mp[question]
# 		nres = [max(x) for x in ans.values() if max(x) > thres]
# 		if len(nres) > 0:
# 			numQAnswered += 1
# 		if len(nres) > 1:
# 			numQAnsweredMultiple += 1

# 	print "Number of questions answered for thres:%s: %d"  % (thres, numQAnswered)
# 	print "Number of questions answered multiple times for thres:%s: %d" % (thres, numQAnsweredMultiple)

maxAnswerScores = []

answerScorer = lambda x: max(x)

#answerScorer = lambda x: max(x)+(sum(x)/len(x))
#answerScorer = lambda x: sum(x[-2:]) / 2


with open("../Results/Threshold-0.45-SINGLEORACLE.txt", "w") as fh:
	for question in mp.keys():
		ans = mp[question]
		# Find the max for the question
		maxAnswerId = -1
		maxAnswer = -1;
		for answerId in ans.keys():
			answers = ans[answerId]
			answers.sort()

			# Normalize the data
			# answers[0] = answers[0] / 0.019807465
			# answers[1] = answers[1] / 0.024667242
			# answers[2] = answers[2] / 0.028944629
			# answers[3] = answers[3] / 0.034828284
			# answers[4] = answers[4] / 0.057759703

			if answerScorer(answers) > maxAnswer:
				maxAnswerId = answerId
				maxAnswer = answerScorer(answers)

		maxAnswerScores.append(maxAnswer)

		if maxAnswer > 0.45:
			fh.write("%d\n" % (maxAnswerId))

print "Average Max Answer: %s" % (sum(maxAnswerScores) / len(maxAnswerScores))
